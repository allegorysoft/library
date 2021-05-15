using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Allegory.Standart.Filter.Concrete;
using Allegory.Standart.Filter.Enums;

namespace Allegory.Standart.Filter.SqlServer.Concrete
{
    public static partial class ConditionSqlExtension
    {
        public static IList<SqlParameter> GetSqlParameters(this Condition condition)
        {
            if (condition == null) return null;

            if (condition.IsColumn)
            {
                if (string.IsNullOrEmpty(condition.ParameterName)) return null;
                if (condition.Operator == Enums.Operator.IsBetween && condition.Value is ICollection)
                {
                    var array = ((ICollection)condition.Value).OfType<object>();
                    return new List<SqlParameter>
                    {
                        new SqlParameter
                        {
                            ParameterName = condition.ParameterName,
                            Value = array.ElementAt(0)
                        },
                        new SqlParameter
                        {
                            ParameterName = "_"+condition.ParameterName,
                            Value = array.ElementAt(1)
                        }
                    };
                }
                else
                    return new List<SqlParameter>
                    {
                        new SqlParameter
                        {
                            ParameterName = condition.ParameterName,
                            Value = condition.Value
                        }
                    };
            }
            else
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                foreach (var _condition in condition.Group)
                {
                    var _sqlParameters = _condition.GetSqlParameters();
                    if (_sqlParameters != null)
                        sqlParameters.AddRange(_sqlParameters);
                }
                return sqlParameters.Count > 0 ? sqlParameters : null;
            }
        }

        public static string GetFilterQuery(this Condition condition, out IList<SqlParameter> sqlParameters, OperatorCombine operatorCombine = OperatorCombine.WithWhere, params string[] columns)
        {
            condition = condition.RemoveConditions(columns);
            sqlParameters = condition.GetSqlParameters();
            return condition.GetFilterQuery(operatorCombine);
        }

        public static string GetFilterQuery<TEntity>(this Condition condition, out IList<SqlParameter> sqlParameters, OperatorCombine operatorCombine = OperatorCombine.WithWhere, bool removeConditions = true, bool convertToValueType = true, params string[] columns)
        {
            if (removeConditions)
                condition = condition.RemoveConditions<TEntity>(columns);
            else if (columns?.Length > 0)
                condition = condition.RemoveConditions(columns);

            if (convertToValueType)
                condition = condition.ConvertToValueType<TEntity>();

            sqlParameters = condition.GetSqlParameters();
            return condition.GetFilterQuery(operatorCombine);
        }
    }
}
