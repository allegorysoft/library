using System.Collections.Generic;
using Allegory.Standard.Filter.Concrete;
using Allegory.Standard.Filter.Enums;
using Allegory.Standard.Filter.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allegory.Standard.Filter.Tests;

[TestClass]
public class ConditionExpressionExtensionTests
{
    [TestMethod]
    public void Equals()
    {
        var conditions = new Condition("column1", Operator.Equals, "some value");

        var expression = conditions.ToLambdaExpression<Sample>();

        Assert.IsTrue(expression.Invoke(new Sample
        {
            column1 = "some value"
        }));
    }

    [TestMethod]
    public void DoesntEquals()
    {
        var conditions = new Condition("column2", Operator.DoesntEquals, 10);

        var expression = conditions.ToLambdaExpression<Sample>();

        Assert.IsTrue(expression.Invoke(new Sample
        {
            column2 = 15
        }));
    }

    [TestMethod]
    public void GreaterThan()
    {
        var conditions = new Condition("column2", Operator.IsGreaterThan, 10);

        var expression = conditions.ToLambdaExpression<Sample>();

        Assert.IsTrue(expression.Invoke(new Sample
        {
            column2 = 15
        }));
    }

    [TestMethod]
    public void GreaterThanOrEqual()
    {
        var conditions = new Condition("column2", Operator.IsGreaterThanOrEqualto, 10);

        var expression = conditions.ToLambdaExpression<Sample>();

        Assert.IsTrue(expression.Invoke(new Sample
        {
            column2 = 10
        }));
    }

    [TestMethod]
    public void LessThan()
    {
        var conditions = new Condition("column2", Operator.IsLessThan, 10);

        var expression = conditions.ToLambdaExpression<Sample>();

        Assert.IsTrue(expression.Invoke(new Sample
        {
            column2 = 5
        }));
    }

    [TestMethod]
    public void LessThanOrEqual()
    {
        var conditions = new Condition("column2", Operator.IsLessThanOrEqualto, 10);

        var expression = conditions.ToLambdaExpression<Sample>();

        Assert.IsTrue(expression.Invoke(new Sample
        {
            column2 = 10
        }));
    }

    [TestMethod]
    public void Between()
    {
        var conditions = new Condition("column2", Operator.IsBetween, new int[]
        {
            10, 15
        });

        var expression = conditions.ToLambdaExpression<Sample>();

        Assert.IsTrue(expression.Invoke(new Sample
        {
            column2 = 12
        }));
    }

    [TestMethod]
    public void Contains()
    {
        var conditions = new Condition("column1", Operator.Contains, "thin");

        var expression = conditions.ToLambdaExpression<Sample>();

        Assert.IsTrue(expression.Invoke(new Sample
        {
            column1 = "something"
        }));
        Assert.IsFalse(expression.Invoke(new Sample
        {
            column1 = "emtpy"
        }));
    }

    [TestMethod]
    public void StartsWith()
    {
        var conditions = new Condition("column1", Operator.StartsWith, "some");

        var expression = conditions.ToLambdaExpression<Sample>();

        Assert.IsTrue(expression.Invoke(new Sample
        {
            column1 = "something"
        }));
        Assert.IsFalse(expression.Invoke(new Sample
        {
            column1 = "emtpy"
        }));
    }

    [TestMethod]
    public void EndsWith()
    {
        var conditions = new Condition("column1", Operator.EndsWith, "ing");

        var expression = conditions.ToLambdaExpression<Sample>();

        Assert.IsTrue(expression.Invoke(new Sample
        {
            column1 = "something"
        }));
        Assert.IsFalse(expression.Invoke(new Sample
        {
            column1 = "emtpy"
        }));
    }

    [TestMethod]
    public void IsNull()
    {
        var conditions = new Condition("column1", Operator.IsNull);

        var expression = conditions.ToLambdaExpression<Sample>();

        Assert.IsTrue(expression.Invoke(new Sample
        {
            column1 = null
        }));
    }

    [TestMethod]
    public void IsNullOrEmpty()
    {
        var conditions = new Condition("column1", Operator.IsNullOrEmpty);

        var expression = conditions.ToLambdaExpression<Sample>();

        Assert.IsTrue(expression.Invoke(new Sample
        {
            column1 = string.Empty
        }));
    }

    [TestMethod]
    public void In()
    {
        var conditions = new Condition("column2", Operator.In, new List<int>
        {
            10,
            12,
            14
        });

        var expression = conditions.ToLambdaExpression<Sample>();

        Assert.IsTrue(expression.Invoke(new Sample
        {
            column2 = 12
        }));
        Assert.IsFalse(expression.Invoke(new Sample
        {
            column2 = 13
        }));
    }

    [TestMethod]
    public void Group()
    {
        var conditions = new Condition
        {
            Group = new List<Condition>
            {
                new Condition
                {
                    Column = "column1",
                    Operator = Operator.Equals,
                    Value = "some value"
                },
                new Condition
                {
                    Column = "column2",
                    Operator = Operator.IsGreaterThan,
                    Value = 20
                },
                new Condition
                {
                    Column = "column3",
                    Operator = Operator.IsLessThan,
                    Value = 20
                }
            }
        };

        var expression = conditions.ToLambdaExpression<Sample>();

        Assert.IsTrue(expression.Invoke(new Sample
        {
            column1 = "some value",
            column2 = 25,
            column3 = 14
        }));
    }

    [TestMethod]
    public void GroupOr()
    {
        var conditions = new Condition
        {
            Group = new List<Condition>
            {
                new Condition
                {
                    Column = "column1",
                    Operator = Operator.Equals,
                    Value = "some value"
                },
                new Condition
                {
                    Column = "column2",
                    Operator = Operator.IsGreaterThan,
                    Value = 20
                }
            }
        };

        var expression = conditions.ToLambdaExpression<Sample>();

        Assert.IsFalse(expression.Invoke(new Sample
        {
            column1 = "some value",
            column2 = 3
        }));

        conditions.GroupOr = true;
        expression = conditions.ToLambdaExpression<Sample>();
        Assert.IsTrue(expression.Invoke(new Sample
        {
            column1 = "some value",
            column2 = 3
        }));
    }

    [TestMethod]
    public void Not()
    {
        var conditions = new Condition("column3", Operator.Equals, 10, true);

        var expression = conditions.ToLambdaExpression<Sample>();

        Assert.IsTrue(expression.Invoke(new Sample
        {
            column2 = 12
        }));
    }

    [TestMethod]
    [ExpectedException(typeof(FilterException))]
    public void ArgumentException()
    {
        var conditions = new Condition();

        var expression = conditions.ToLambdaExpression<Sample>();
    }

    [TestMethod]
    public void ShouldRenameParameterNames()
    {
        var conditions = new Condition
        {
            Group = new List<Condition>
            {
                new Condition
                {
                    Column = "column1",
                    Operator = Operator.Equals,
                    Value = "some value"
                },
                new Condition
                {
                    Column = "column2",
                    Operator = Operator.IsGreaterThan,
                    Value = 20
                },
                new Condition
                {
                    Group = new List<Condition>
                    {
                        new Condition
                        {
                            Column = "column1",
                            Operator = Operator.Equals,
                            Value = "some value"
                        },
                        new Condition
                        {
                            Column = "column2",
                            Operator = Operator.IsGreaterThan,
                            Value = 20
                        },
                        new Condition
                        {
                            Column = "column3",
                            Operator = Operator.IsNull
                        }
                    }
                }
            }
        };

        var expression = conditions.GetFilterQuery<Sample>(out var parameters);

        Assert.AreEqual(4, parameters.Count);
        for (var i = 0; i < parameters.Count; i++)
        {
            Assert.IsTrue(parameters.ContainsKey("P-" + (i + 1)));
        }
    }
}