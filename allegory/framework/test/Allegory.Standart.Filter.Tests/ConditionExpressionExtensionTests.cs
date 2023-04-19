using System.Collections.Generic;
using Allegory.Standart.Filter.Concrete;
using Allegory.Standart.Filter.Enums;
using Allegory.Standart.Filter.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allegory.Standart.Filter.Tests
{
    [TestClass]
    public class ConditionExpressionExtensionTests
    {
        Condition conditions;

        [TestMethod]
        public void Equals()
        {
            conditions = new Condition("column1", Operator.Equals, "some value");

            var expression = conditions.ToExpression<Sample>();

            Assert.IsTrue(expression.Compile().Invoke(new Sample
            {
                column1 = "some value"
            }));
        }

        [TestMethod]
        public void DoesntEquals()
        {
            conditions = new Condition("column2", Operator.DoesntEquals, 10);

            var expression = conditions.ToExpression<Sample>();

            Assert.IsTrue(expression.Compile().Invoke(new Sample
            {
                column2 = 15
            }));
        }

        [TestMethod]
        public void GreaterThan()
        {
            conditions = new Condition("column2", Operator.IsGreaterThan, 10);

            var expression = conditions.ToExpression<Sample>();

            Assert.IsTrue(expression.Compile().Invoke(new Sample
            {
                column2 = 15
            }));
        }

        [TestMethod]
        public void GreaterThanOrEqual()
        {
            conditions = new Condition("column2", Operator.IsGreaterThanOrEqualto, 10);

            var expression = conditions.ToExpression<Sample>();

            Assert.IsTrue(expression.Compile().Invoke(new Sample
            {
                column2 = 10
            }));
        }

        [TestMethod]
        public void LessThan()
        {
            conditions = new Condition("column2", Operator.IsLessThan, 10);

            var expression = conditions.ToExpression<Sample>();

            Assert.IsTrue(expression.Compile().Invoke(new Sample
            {
                column2 = 5
            }));
        }

        [TestMethod]
        public void LessThanOrEqual()
        {
            conditions = new Condition("column2", Operator.IsLessThanOrEqualto, 10);

            var expression = conditions.ToExpression<Sample>();

            Assert.IsTrue(expression.Compile().Invoke(new Sample
            {
                column2 = 10
            }));
        }

        [TestMethod]
        public void Between()
        {
            conditions = new Condition("column2", Operator.IsBetween, new int[] { 10, 15 });

            var expression = conditions.ToExpression<Sample>();

            Assert.IsTrue(expression.Compile().Invoke(new Sample
            {
                column2 = 12
            }));
        }

        [TestMethod]
        public void Contains()
        {
            conditions = new Condition("column1", Operator.Contains, "thin");

            var expression = conditions.ToExpression<Sample>();

            Assert.IsTrue(expression.Compile().Invoke(new Sample
            {
                column1 = "something"
            }));
            Assert.IsFalse(expression.Compile().Invoke(new Sample
            {
                column1 = "emtpy"
            }));
        }

        [TestMethod]
        public void StartsWith()
        {
            conditions = new Condition("column1", Operator.StartsWith, "some");

            var expression = conditions.ToExpression<Sample>();

            Assert.IsTrue(expression.Compile().Invoke(new Sample
            {
                column1 = "something"
            }));
            Assert.IsFalse(expression.Compile().Invoke(new Sample
            {
                column1 = "emtpy"
            }));
        }

        [TestMethod]
        public void EndsWith()
        {
            conditions = new Condition("column1", Operator.EndsWith, "ing");

            var expression = conditions.ToExpression<Sample>();

            Assert.IsTrue(expression.Compile().Invoke(new Sample
            {
                column1 = "something"
            }));
            Assert.IsFalse(expression.Compile().Invoke(new Sample
            {
                column1 = "emtpy"
            }));
        }

        [TestMethod]
        public void IsNull()
        {
            conditions = new Condition("column1", Operator.IsNull);

            var expression = conditions.ToExpression<Sample>();

            Assert.IsTrue(expression.Compile().Invoke(new Sample
            {
                column1 = null
            }));
        }

        [TestMethod]
        public void IsNullOrEmpty()
        {
            conditions = new Condition("column1", Operator.IsNullOrEmpty);

            var expression = conditions.ToExpression<Sample>();

            Assert.IsTrue(expression.Compile().Invoke(new Sample
            {
                column1 = string.Empty
            }));
        }

        [TestMethod]
        public void In()
        {
            conditions = new Condition("column2", Operator.In, new List<int> { 10, 12, 14 });

            var expression = conditions.ToExpression<Sample>();

            Assert.IsTrue(expression.Compile().Invoke(new Sample
            {
                column2 = 12
            }));
            Assert.IsFalse(expression.Compile().Invoke(new Sample
            {
                column2 = 13
            }));
        }

        [TestMethod]
        public void Group()
        {
            conditions = new Condition
            {
                Group = new List<Condition>
                {
                    new Condition
                    {
                        Column="column1",
                        Operator=Operator.Equals,
                        Value="some value"
                    },
                    new Condition
                    {
                        Column="column2",
                        Operator=Operator.IsGreaterThan,
                        Value=20
                    },
                    new Condition
                    {
                        Column="column3",
                        Operator=Operator.IsLessThan,
                        Value=20
                    }
                }
            };

            var expression = conditions.ToExpression<Sample>();

            Assert.IsTrue(expression.Compile().Invoke(new Sample
            {
                column1 = "some value",
                column2 = 25,
                column3 = 14
            }));
        }

        [TestMethod]
        public void GroupOr()
        {
            conditions = new Condition
            {
                Group = new List<Condition>
                {
                    new Condition
                    {
                        Column="column1",
                        Operator=Operator.Equals,
                        Value="some value"
                    },
                    new Condition
                    {
                        Column="column2",
                        Operator=Operator.IsGreaterThan,
                        Value=20
                    }
                }
            };

            var expression = conditions.ToExpression<Sample>();

            Assert.IsFalse(expression.Compile().Invoke(new Sample
            {
                column1 = "some value",
                column2 = 3
            }));

            conditions.GroupOr = true;
            expression = conditions.ToExpression<Sample>();
            Assert.IsTrue(expression.Compile().Invoke(new Sample
            {
                column1 = "some value",
                column2 = 3
            }));
        }

        [TestMethod]
        public void Not()
        {
            conditions = new Condition("column3", Operator.Equals, 10, true);

            var expression = conditions.ToExpression<Sample>();

            Assert.IsTrue(expression.Compile().Invoke(new Sample
            {
                column2 = 12
            }));
        }

        [TestMethod]
        [ExpectedException(typeof(FilterException))]
        public void ArgumentException()
        {
            conditions = new Condition();

            var expression = conditions.ToExpression<Sample>();
        }
    }
}
