using System.Collections.Generic;
using Allegory.Standard.Filter.Enums;
using Allegory.Standard.Filter.Properties;

namespace Allegory.Standard.Filter.Concrete;

public sealed class Sort
{
    #region Constructors

    public Sort() {}
    public Sort(string column, OrderDirection orderDirection = OrderDirection.Asc)
    {
        Column = column;
        OrderDirection = orderDirection;
    }
    public Sort(IList<Sort> sorts)
    {
        Group = sorts;
    }

    #endregion

    #region Properties

    public string Column { get; set; }
    public OrderDirection OrderDirection { get; set; }

    public IList<Sort> Group { get; set; }

    public bool IsColumn => Group == null || Group.Count == 0;
    public bool IsGroup => !IsColumn;

    #endregion

    #region Methods

    public void ValidateColumn()
    {
        if (IsColumn)
        {
            if (string.IsNullOrEmpty(Column))
                throw new FilterException(Resource.ColumnNullError);
        }
    }

    #endregion

    public override string ToString()
    {
        if (IsGroup)
            return string.Join(", ", Group);

        ValidateColumn();
        string[] column = Column.Replace("[", "[[").Replace("]", "]]").Split('.');
        string columnName = column.Length > 1
            ? "[" + column[0] + "].[" + column[1] + "]"
            : "[" + Column.Replace("[", "[[").Replace("]", "]]") + "]";

        return string.Format($"{columnName} {OrderDirection.ToString()}");
    }
}