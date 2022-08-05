using System;
using System.Collections.Generic;
using System.Linq;
using Allegory.Standart.Filter.Concrete;
using Allegory.Standart.Filter.Enums;
using Allegory.Standart.Filter.Properties;
using DevExpress.Data.Filtering;

namespace Allegory.Filter.Dx.Concrete
{
    public static class ConditionDevExpressExtension
    {
        public static Condition GetConditions(this CriteriaOperator criter)
        {
            GroupOperator group = criter as GroupOperator;
            if (group is null)
                return GetCondition(criter);
            else
            {
                List<Condition> conditions = new List<Condition>();
                foreach (var condition in group.Operands)
                    conditions.Add(condition.GetConditions());
                return conditions.Count > 0 ? new Condition(conditions, group.OperatorType == GroupOperatorType.Or, false) : null;
            }
        }
        private static Condition GetCondition(CriteriaOperator criter)
        {
            if (criter is null) return null;
            switch (criter?.GetType().Name)
            {
                case nameof(BinaryOperator): return Binary(criter as BinaryOperator);
                case nameof(FunctionOperator): return Function(criter as FunctionOperator);
                case nameof(BetweenOperator): return Between(criter as BetweenOperator);
                case nameof(UnaryOperator): return Unary(criter as UnaryOperator);
                case nameof(InOperator): return In(criter as InOperator);
                default:
                    throw new FilterException(string.Format(Resource.UnsupportedOperatorError, criter?.GetType().Name, "condition"));
            }
        }
        private static Condition Binary(BinaryOperator binaryOperator)
        {
            OperandProperty opProperty = binaryOperator.LeftOperand as OperandProperty;
            OperandValue opValue = binaryOperator.RightOperand as OperandValue;
            if (ReferenceEquals(binaryOperator, null) || ReferenceEquals(opValue, null)) return null;

            switch (binaryOperator.OperatorType)
            {
                case BinaryOperatorType.Equal:
                    return new Condition(opProperty.PropertyName, Operator.Equals, opValue.Value);
                case BinaryOperatorType.NotEqual:
                    return new Condition(opProperty.PropertyName, Operator.DoesntEquals, opValue.Value);
                case BinaryOperatorType.Greater:
                    return new Condition(opProperty.PropertyName, Operator.IsGreaterThan, opValue.Value);
                case BinaryOperatorType.Less:
                    return new Condition(opProperty.PropertyName, Operator.IsLessThan, opValue.Value);
                case BinaryOperatorType.LessOrEqual:
                    return new Condition(opProperty.PropertyName, Operator.IsLessThanOrEqualto, opValue.Value);
                case BinaryOperatorType.GreaterOrEqual:
                    return new Condition(opProperty.PropertyName, Operator.IsGreaterThanOrEqualto, opValue.Value);
                default:
                    throw new FilterException(string.Format(Resource.UnsupportedOperatorError, binaryOperator.OperatorType,"binary"));
            }
        }
        private static Condition Function(FunctionOperator functionOperator)
        {
            if (ReferenceEquals(functionOperator, null)) return null;

            OperandProperty opProperty = functionOperator.Operands.Count == 3
                ? functionOperator.Operands[1] as OperandProperty : functionOperator.Operands[0] as OperandProperty;

            OperandValue opValue = functionOperator.Operands.Count > 1
                ? functionOperator.Operands.Count == 3
                        ? functionOperator.Operands[2] as OperandValue
                        : functionOperator.Operands[1] as OperandValue
                : null;

            switch (functionOperator.OperatorType)
            {
                case FunctionOperatorType.Contains:
                    return new Condition(opProperty.PropertyName, Operator.Contains, opValue.Value);
                case FunctionOperatorType.StartsWith:
                    return new Condition(opProperty.PropertyName, Operator.StartsWith, opValue.Value);
                case FunctionOperatorType.EndsWith:
                    return new Condition(opProperty.PropertyName, Operator.EndsWith, opValue.Value);
                case FunctionOperatorType.IsNullOrEmpty:
                    return new Condition(opProperty.PropertyName, Operator.IsNullOrEmpty);
                default:
                    throw new FilterException(string.Format(Resource.UnsupportedOperatorError, functionOperator.OperatorType,"function"));
            }
        }
        private static Condition Between(BetweenOperator betweenOperator)
        {
            OperandProperty opProperty = betweenOperator.TestExpression as OperandProperty;
            OperandValue opValue = betweenOperator.BeginExpression as OperandValue;
            OperandValue opValue2 = betweenOperator.EndExpression as OperandValue;

            var listType = typeof(List<>).MakeGenericType(opValue.Value.GetType());
            var list = Activator.CreateInstance(listType, 2);
            var method = listType.GetMethod("Add");

            method.Invoke(list, new object[] { opValue.Value });
            method.Invoke(list, new object[] { opValue2.Value });

            return new Condition(opProperty.PropertyName, Operator.IsBetween, list);
        }
        private static Condition Unary(UnaryOperator unaryOperator)
        {
            if (unaryOperator.OperatorType == UnaryOperatorType.IsNull)
            {
                OperandProperty prop = unaryOperator.Operand as OperandProperty;
                return new Condition(prop.PropertyName, Operator.IsNull);
            }
            else if (unaryOperator.OperatorType == UnaryOperatorType.Not)
            {
                Condition condition = GetConditions(unaryOperator.Operand);
                if (condition != null)
                    condition.Not = true;
                return condition;
            }
            throw new FilterException(string.Format(Resource.UnsupportedOperatorError, unaryOperator.OperatorType, "unary"));
        }
        private static Condition In(InOperator inOperator)
        {
            var values = (inOperator.Operands as CriteriaOperatorCollection).OfType<ConstantValue>().Select(x => x.Value).ToList();

            var listType = typeof(List<>).MakeGenericType(values.FirstOrDefault().GetType());
            var list = Activator.CreateInstance(listType, values.Count);
            var method = listType.GetMethod("Add");
            for (int i = 0; i < values.Count; i++)
            {
                method.Invoke(list, new object[] { values[i] });
            }

            OperandProperty property = inOperator.LeftOperand as OperandProperty;
            return new Condition(property.PropertyName, Operator.In, list);
        }
    }
}
