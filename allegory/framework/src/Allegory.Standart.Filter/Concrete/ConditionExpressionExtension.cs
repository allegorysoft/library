using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Allegory.Standart.Filter.Properties;

namespace Allegory.Standart.Filter.Concrete
{
    public static partial class ConditionExtension
    {
        public static Expression<Func<TEntity, bool>> ToExpression<TEntity>(this Condition condition)
        {
            if (condition == null) return null;

            var parameterExpression = Expression.Parameter(typeof(TEntity));

            condition = condition.ConvertToValueType<TEntity>();

            var expression = GetExpression(parameterExpression, condition);

            return expression == null ? null : GetLambda<TEntity>(expression, parameterExpression);
        }

        [Obsolete("This extension method is obsolete and will be removed in a future version. Use ToExpression instead.")]
        public static Expression<Func<TEntity, bool>> GetLambdaExpression<TEntity>(this Condition condition)
        {
            if (condition == null) return null;

            var parameterExpression = Expression.Parameter(typeof(TEntity));

            condition = condition.ConvertToValueType<TEntity>();

            var expression = GetExpression(parameterExpression, condition);

            return expression == null ? null : GetLambda<TEntity>(expression, parameterExpression);
        }

        private static Expression<Func<TEntity, bool>> GetLambda<TEntity>(
            Expression expression,
            ParameterExpression parameterExpression)
        {
            return (Expression<Func<TEntity, bool>>)Expression.Lambda(expression, parameterExpression);
        }

        private static Expression GetExpression(ParameterExpression parameterExpression, Condition condition)
        {
            Expression expression = Expression.Default(typeof(object));

            if (condition.IsColumn)
            {
                SetExpression(ref expression, parameterExpression, condition);
            }
            else
            {
                List<Expression> expressions = new List<Expression>();
                foreach (var _condition in condition.Group)
                    expressions.Add(GetExpression(parameterExpression, _condition));

                if (expressions.Count < 2)
                    expression = expressions.FirstOrDefault();
                else
                    for (int i = 0; i < expressions.Count - 1; i++)
                    {
                        if (i == 0)
                        {
                            expression = Expression.MakeBinary(
                                condition.GroupOr ? ExpressionType.OrElse : ExpressionType.AndAlso
                                , expressions[i]
                                , expressions[i + 1]);
                        }
                        else
                        {
                            expression = Expression.MakeBinary(
                                condition.GroupOr ? ExpressionType.OrElse : ExpressionType.AndAlso
                                , expression
                                , expressions[i + 1]
                            );
                        }
                    }

                if (condition.Not)
                    expression = Expression.Not(expression);
            }

            return expression;
        }

        private static void SetExpression(ref Expression expression, ParameterExpression parameterExpression,
            Condition condition)
        {
            if (condition.Operator == Enums.Operator.Equals)
                expression = Expression.Equal(Expression.PropertyOrField(parameterExpression, condition.Column),
                    Expression.Constant(condition.Value,
                        Expression.PropertyOrField(parameterExpression, condition.Column).Type));
            else if (condition.Operator == Enums.Operator.DoesntEquals)
                expression = Expression.NotEqual(Expression.PropertyOrField(parameterExpression, condition.Column),
                    Expression.Constant(condition.Value,
                        Expression.PropertyOrField(parameterExpression, condition.Column).Type));

            else if (condition.Operator == Enums.Operator.IsGreaterThan)
                SetComparableExpression(ref expression, parameterExpression, condition, ExpressionType.GreaterThan);
            else if (condition.Operator == Enums.Operator.IsGreaterThanOrEqualto)
                SetComparableExpression(ref expression, parameterExpression, condition,
                    ExpressionType.GreaterThanOrEqual);
            else if (condition.Operator == Enums.Operator.IsLessThan)
                SetComparableExpression(ref expression, parameterExpression, condition, ExpressionType.LessThan);
            else if (condition.Operator == Enums.Operator.IsLessThanOrEqualto)
                SetComparableExpression(ref expression, parameterExpression, condition, ExpressionType.LessThanOrEqual);

            else if (condition.Operator == Enums.Operator.IsBetween)
                SetBetweenExpression(ref expression, parameterExpression, condition);

            else if (condition.Operator == Enums.Operator.Contains)
                SetStringExpression(ref expression, parameterExpression, condition, nameof(string.Contains));
            else if (condition.Operator == Enums.Operator.StartsWith)
                SetStringExpression(ref expression, parameterExpression, condition, nameof(string.StartsWith));
            else if (condition.Operator == Enums.Operator.EndsWith)
                SetStringExpression(ref expression, parameterExpression, condition, nameof(string.EndsWith));

            else if (condition.Operator == Enums.Operator.IsNull)
                expression = Expression.Equal(Expression.PropertyOrField(parameterExpression, condition.Column),
                    Expression.Constant(null));
            else if (condition.Operator == Enums.Operator.IsNullOrEmpty)
            {
                expression = Expression.Call(
                    typeof(string).GetMethod(nameof(string.IsNullOrEmpty), new Type[] { typeof(string) }),
                    Expression.PropertyOrField(parameterExpression, condition.Column));
            }

            else if (condition.Operator == Enums.Operator.In)
            {
                MemberExpression memberExpression = Expression.PropertyOrField(parameterExpression, condition.Column);

                expression = Expression.Call(
                    typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                        .Single(m => m.Name == nameof(Enumerable.Contains)
                                     && m.GetParameters().Count() == 2)
                        .MakeGenericMethod(memberExpression.Type)
                    , new Expression[]
                    {
                        Expression.Constant(condition.Value),
                        memberExpression
                    });
            }

            if (condition.Not)
                expression = Expression.Not(expression);
        }

        private static void SetComparableExpression(ref Expression expression, ParameterExpression parameterExpression,
            Condition condition, ExpressionType expressionType)
        {
            if (typeof(IComparable).IsAssignableFrom(condition.Value.GetType()))
            {
                if (condition.Value.GetType().Equals(typeof(string)))
                {
                    expression = Expression.Call(
                        Expression.PropertyOrField(parameterExpression, condition.Column),
                        condition.Value.GetType().GetMethod(nameof(IComparable.CompareTo), new[] { typeof(object) }),
                        Expression.Constant(condition.Value));
                    expression = Expression.MakeBinary(expressionType, expression, Expression.Constant(0));
                }
                else
                    expression = Expression.MakeBinary(expressionType,
                        Expression.PropertyOrField(parameterExpression, condition.Column),
                        Expression.Constant(condition.Value,
                            Expression.PropertyOrField(parameterExpression, condition.Column).Type));
            }
            else
                throw new FilterException(string.Format(Resource.ImplementError, condition.Value.GetType(),
                    nameof(IComparable)));
        }

        private static void SetBetweenExpression(ref Expression expression, ParameterExpression parameterExpression,
            Condition condition)
        {
            var array = ((ICollection)condition.Value).OfType<object>();
            var startValue = Expression.Constant(array.ElementAt(0),
                Expression.PropertyOrField(parameterExpression, condition.Column).Type);
            var endValue = Expression.Constant(array.ElementAt(1),
                Expression.PropertyOrField(parameterExpression, condition.Column).Type);

            if (typeof(IComparable).IsAssignableFrom(array.ElementAt(0).GetType()))
            {
                if (condition.Value is ICollection<string>)
                {
                    var start = Expression.Call(
                        Expression.PropertyOrField(parameterExpression, condition.Column),
                        typeof(string).GetMethod(nameof(string.CompareTo), new[] { typeof(string) }),
                        startValue);
                    var end = Expression.Call(
                        Expression.PropertyOrField(parameterExpression, condition.Column),
                        typeof(string).GetMethod(nameof(string.CompareTo), new[] { typeof(string) }),
                        endValue);
                    expression = Expression.AndAlso(Expression.GreaterThanOrEqual(start, Expression.Constant(0)),
                        Expression.LessThanOrEqual(end, Expression.Constant(0)));
                }
                else
                    expression = Expression.AndAlso(
                        Expression.GreaterThanOrEqual(Expression.PropertyOrField(parameterExpression, condition.Column),
                            startValue),
                        Expression.LessThanOrEqual(Expression.PropertyOrField(parameterExpression, condition.Column),
                            endValue)
                    );
            }
            else
                throw new FilterException(string.Format(Resource.ImplementError, condition.Value.GetType(),
                    nameof(IComparable)));
        }

        private static void SetStringExpression(ref Expression expression, ParameterExpression parameterExpression,
            Condition condition, string methodName)
        {
            expression = Expression.Call(Expression.PropertyOrField(parameterExpression, condition.Column),
                typeof(string).GetMethod(methodName, new[] { typeof(string) }),
                Expression.Constant(condition.Value));
        }

        //Expression to Condition code here
    }
}