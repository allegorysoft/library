using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Allegory.Standart.Filter.Enums;
using Allegory.Standart.Filter.Properties;

namespace Allegory.Standart.Filter.Concrete
{
    public static partial class ConditionExtension
    {
        public static IDictionary<string, object> GetDictionaries(this Condition condition)
        {
            if (condition == null) return null;

            if (condition.IsColumn)
            {
                if (string.IsNullOrEmpty(condition.ParameterName)) return null;
                if (condition.Operator == Enums.Operator.IsBetween && condition.Value is ICollection)
                {
                    var array = ((ICollection)condition.Value).OfType<object>();
                    return new Dictionary<string, object>
                    {
                        { condition.ParameterName, array.ElementAt(0) },
                        { "_" + condition.ParameterName, array.ElementAt(1) }
                    };
                }
                else
                    return new Dictionary<string, object>
                    {
                        { condition.ParameterName, condition.Value },
                    };
            }
            else
            {
                List<KeyValuePair<string, object>> dictionaryList = new List<KeyValuePair<string, object>>();
                foreach (var _condition in condition.Group)
                {
                    var _dictionaryList = _condition.GetDictionaries();
                    if (_dictionaryList != null)
                        dictionaryList.AddRange(_dictionaryList);
                }

                return dictionaryList.Count > 0 ? dictionaryList.ToDictionary(k => k.Key, k => k.Value) : null;
            }
        }

        public static string GetFilterQuery(this Condition condition,
            OperatorCombine operatorCombine = OperatorCombine.WithWhere)
        {
            if (condition == null || string.IsNullOrEmpty(condition.ToString())) return null;
            string filterQuery = condition.ToString();
            switch (operatorCombine)
            {
                case OperatorCombine.WithNone:
                    break;
                case OperatorCombine.WithWhere:
                    filterQuery = filterQuery.Insert(0, " WHERE ");
                    break;
                case OperatorCombine.WithAnd:
                    filterQuery = filterQuery.Insert(0, " AND ");
                    break;
                case OperatorCombine.WithOr:
                    filterQuery = filterQuery.Insert(0, " OR ");
                    break;
                case OperatorCombine.WithAndBrackets:
                    filterQuery = filterQuery.Insert(0, " AND (");
                    filterQuery += ")";
                    break;
                case OperatorCombine.WithOrBrackets:
                    filterQuery = filterQuery.Insert(0, " OR (");
                    filterQuery += ")";
                    break;
            }

            return filterQuery;
        }

        public static string GetFilterQuery(this Condition condition, out IDictionary<string, object> dictionaries,
            OperatorCombine operatorCombine = OperatorCombine.WithWhere, params string[] columns)
        {
            condition = condition.RemoveConditions(columns);
            dictionaries = condition.GetDictionaries();
            return condition.GetFilterQuery(operatorCombine);
        }

        public static string GetFilterQuery<TEntity>(this Condition condition,
            out IDictionary<string, object> dictionaries, OperatorCombine operatorCombine = OperatorCombine.WithWhere,
            bool removeConditions = true, bool convertToValueType = true, params string[] columns)
        {
            if (removeConditions)
                condition = condition.RemoveConditions<TEntity>(columns);
            else if (columns?.Length > 0)
                condition = condition.RemoveConditions(columns);

            if (convertToValueType)
                condition = condition.ConvertToValueType<TEntity>();

            dictionaries = condition.GetDictionaries();
            return condition.GetFilterQuery(operatorCombine);
        }

        public static Condition RemoveConditions(this Condition condition, params string[] columns)
        {
            if (condition == null || columns == null || columns.Length == 0) return condition;

            if (condition.IsColumn)
            {
                return columns.Contains(condition.Column) ? null : condition;
            }
            else
            {
                List<Condition> conditions = new List<Condition>();
                for (int i = 0; i < condition.Group.Count; i++)
                {
                    condition.Group[i] = condition.Group[i].RemoveConditions(columns);
                    if (condition.Group[i] != null)
                        conditions.Add(condition.Group[i]);
                }

                return conditions.Count > 0 ? new Condition(conditions, condition.GroupOr, condition.Not) : null;
            }
        }

        public static Condition RemoveConditions<TEntity>(this Condition condition, params string[] columns)
        {
            if (condition == null) return condition;

            if (condition.IsColumn)
            {
                return typeof(TEntity).GetProperty(condition.Column) == null
                    ? throw new FilterException(string.Format(Resource.MemberOfTypeError, condition.Column,
                        typeof(TEntity).FullName))
                    : condition.RemoveConditions(columns);
            }
            else
            {
                List<Condition> conditions = new List<Condition>();
                for (int i = 0; i < condition.Group.Count; i++)
                {
                    condition.Group[i] = condition.Group[i].RemoveConditions<TEntity>(columns);
                    if (condition.Group[i] != null)
                        conditions.Add(condition.Group[i]);
                }

                return conditions.Count > 0 ? new Condition(conditions, condition.GroupOr, condition.Not) : null;
            }
        }

        public static Condition ConvertToValueType<TEntity>(this Condition condition)
        {
            if (condition == null) return condition;

            if (condition.IsColumn)
            {
                return CheckColumn<TEntity>(condition);
            }

            List<Condition> conditions = new List<Condition>();
            for (int i = 0; i < condition.Group.Count; i++)
            {
                condition.Group[i] = condition.Group[i].ConvertToValueType<TEntity>();
                if (condition.Group[i] != null)
                    conditions.Add(condition.Group[i]);
            }

            return conditions.Count > 0 ? new Condition(conditions, condition.GroupOr, condition.Not) : null;
        }

        private static Condition CheckColumn<TEntity>(Condition condition)
        {
            ParseCommaSeparatedStringToArray(condition);
            condition.ValidateColumn();
            var property = typeof(TEntity).GetProperty(condition.Column);

            if (property == null || condition.Value == null) return condition;

            var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            if (condition.Value is ICollection)
            {
                if (!typeof(ICollection<>).MakeGenericType(property.PropertyType)
                        .IsAssignableFrom(condition.Value.GetType()))
                {
                    var array = ((ICollection)condition.Value).OfType<object>();
                    var listType = typeof(List<>).MakeGenericType(property.PropertyType);
                    var list = Activator.CreateInstance(listType, array.Count());
                    var method = listType.GetMethod("Add");
                    for (int i = 0; i < array.Count(); i++)
                        method.Invoke(list,
                            new object[]
                            {
                                array.ElementAt(i) == null ? null : GetValue(array.ElementAt(i), propertyType)
                            });
                    condition.Value = list;
                }
            }
            else if (condition.Value.GetType() != propertyType)
                condition.Value = GetValue(condition.Value, propertyType);

            return condition;
        }

        private static void ParseCommaSeparatedStringToArray(Condition condition)
        {
            if (condition.Value is string &&
                (condition.Operator == Operator.In || condition.Operator == Operator.IsBetween))
                condition.Value = condition.Value
                    .ToString()
                    .Split(new[] { ", " }, StringSplitOptions.None)
                    .ToArray();
        }

        private static object GetValue(object value, Type propertyType)
        {
            if (propertyType.IsEnum)
            {
                return Enum.Parse(propertyType, value.ToString());
            }
            else
            {
                return Convert.ChangeType(value, propertyType);
            }
        }
    }
}