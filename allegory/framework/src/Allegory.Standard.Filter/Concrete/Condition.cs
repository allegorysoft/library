using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Allegory.Standard.Filter.Enums;
using Allegory.Standard.Filter.Properties;

namespace Allegory.Standard.Filter.Concrete;

public sealed class Condition
{
    #region Fields

    private string _parameterName;
    private ObservableCollection<Condition> _group;

    #endregion

    #region Properties

    public string Column { get; set; }
    public Operator Operator { get; set; }
    public object Value { get; set; }
    public bool Not { get; set; }

    public IList<Condition> Group
    {
        get { return _group; }
        set
        {
            if (value != null)
            {
                for (int i = 0; i < value.Count; i++)
                    value[i].Parent = this;
                _group = new ObservableCollection<Condition>(value);
                _group.CollectionChanged += Group_CollectionChanged;
            }
            else
            {
                if (_group != null)
                    _group.CollectionChanged -= Group_CollectionChanged;
                _group = null;
            }
        }
    }

    public bool GroupOr { get; set; }

    public string ParameterName
    {
        get
        {
            if (string.IsNullOrEmpty(_parameterName) && IsColumn && Value != null)
                _parameterName = Guid.NewGuid().ToString().Replace("-", "");

            return _parameterName;
        }
    }

    private Condition Parent { get; set; }

    public bool IsColumn => Group == null || Group.Count == 0;
    public bool IsGroup => Group != null && Group.Count > 0;

    #endregion

    #region Constructors

    public Condition() {}

    public Condition(string column, Operator @operator, object value = null, bool not = false)
    {
        Column = column;
        Operator = @operator;
        Value = value;
        Not = not;
    }

    public Condition(IList<Condition> conditions, bool groupOr, bool not)
    {
        this.Group = conditions;
        this.GroupOr = groupOr;
        this.Not = not;
    }

    #endregion

    #region Methods

    public void ValidateColumn()
    {
        if (IsColumn)
        {
            if (string.IsNullOrEmpty(Column))
                throw new FilterException(Resource.ColumnNullError);
            if (Value == null && !(Operator == Operator.IsNull || Operator == Operator.IsNullOrEmpty))
                throw new FilterException(Resource.ValueNullError);
            if (Operator == Operator.IsBetween
                && !(Value is ICollection && ((ICollection) Value).Count == 2))
                throw new FilterException(Resource.BetweenValueError);
            if (Operator == Operator.In
                && !(Value is ICollection && ((ICollection) Value).Count > 0))
                throw new FilterException(Resource.InValueError);
        }
    }

    private void Group_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action != NotifyCollectionChangedAction.Add) return;

        for (int i = e.NewStartingIndex; i < _group.Count; i++)
            _group[i].Parent = this;
    }

    #endregion

    public override string ToString()
    {
        if (IsGroup)
            return string.Concat((Not ? "NOT" : ""), (Parent == null && Not == false ? string.Empty : "("),
                string.Join((GroupOr ? " OR " : " AND "), Group),
                (Parent == null && Not == false ? string.Empty : ")"));

        ValidateColumn();
        string[] column = Column.Replace("[", "[[").Replace("]", "]]").Split('.');
        string columnName = column.Length > 1
            ? "[" + column[0] + "].[" + column[1] + "]"
            : "[" + Column.Replace("[", "[[").Replace("]", "]]") + "]";
        string filter = string.Concat((Not ? "NOT " : string.Empty), columnName);

        switch (Operator)
        {
            case Operator.Equals:
                filter += "=";
                break;
            case Operator.DoesntEquals:
                filter += "<>";
                break;
            case Operator.IsGreaterThan:
                filter += ">";
                break;
            case Operator.IsGreaterThanOrEqualto:
                filter += ">=";
                break;
            case Operator.IsLessThan:
                filter += "<";
                break;
            case Operator.IsLessThanOrEqualto:
                filter += "<=";
                break;
            case Operator.In://Working with dapper
                filter += " IN ";
                break;
            case Operator.IsBetween:
                filter += " BETWEEN @" + ParameterName + " AND @_" + ParameterName;
                return filter;
            case Operator.Contains:
                filter += " LIKE '%'+@" + ParameterName + "+'%'";
                return filter;
            case Operator.StartsWith:
                filter += " LIKE @" + ParameterName + "+'%'";
                return filter;
            case Operator.EndsWith:
                filter += " LIKE '%'+@" + ParameterName;
                return filter;
            case Operator.IsNull:
                filter += " IS NULL";
                return filter;
            case Operator.IsNullOrEmpty:
                return string.Concat((Not ? " NOT " : string.Empty), "NULLIF(", columnName, ",'') IS NULL");
        }

        filter += "@" + ParameterName;
        return filter;
    }

    internal void RenameParameters()
    {
        var p = this;
        while (p.Parent != null)
        {
            p = p.Parent;
        }

        var counter = 1;
        RenameParameters(p, ref counter);
    }

    private void RenameParameters(Condition condition, ref int c)
    {
        if (condition.IsGroup)
        {
            foreach (var item in condition.Group)
            {
                RenameParameters(item, ref c);
            }
        }
        else
        {
            if (condition.IsColumn && condition.Value != null)
            {
                condition._parameterName = "P-" + c++;
            }
        }
    }
}