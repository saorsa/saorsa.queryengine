using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework.Internal;

namespace Saorsa.QueryEngine.Tests;

/// <summary>
/// Tests related to the <seealso cref="T:Saorsa.QueryEngine.ExpressionExtensions"/> class.
/// </summary>
public class ExpressionExtensionTests
{
    [Test]
    public void TestBasicProperty()
    {
        var paramType = typeof(TestDummyClass);
        var param = Expression.Parameter(paramType);
        var instancePublic = BindingFlags.Public | BindingFlags.Instance;

        paramType.GetProperties(instancePublic)
            .ToList()
            .ForEach(mi =>
            {
                var memberExpression = param.GetPropertyExpression(mi.Name);
                Assert.NotNull(memberExpression,
                    $"GetPropertyExpression creates a member expression for property [{mi.Name}]");
            });
    }

    [Test]
    public void TestNestedProperty()
    {
        var nestedPropertyPath =
            $"{nameof(TestDummyClass.NestedDummy)}.{nameof(TestDummyClass.NestedDummy)}." +
            $"{nameof(TestDummyClass.NestedDummy)}.{nameof(TestDummyClass.EnumValue)}";
        var param = Expression.Parameter(typeof(TestDummyClass));
        var propExpression = param.GetPropertyExpression(nestedPropertyPath);
        Assert.NotNull(
            propExpression,
            $"GetPropertyExpression returns a valid expression for nested property sequences");
    }

    [Test]
    public void TestCountExpression()
    {
        var param = Expression.Parameter(typeof(TestDummyClass));
        var propExpression = param.GetPropertyExpression(nameof(TestDummyClass.ArrayOfIntegers));
        var countExpression = propExpression.ToCountExpression<TestDummyClass>();
        Assert.NotNull(
            countExpression,
            $"ToCountExpression<int> returns a valid expression");
        countExpression = propExpression.ToCountExpression<TestDummyClass>();
        Assert.NotNull(
            countExpression,
            $"ToCountExpression<int> returns a valid expression");

        propExpression = param.GetPropertyExpression(nameof(TestDummyClass.EnumerableOfIntegers));
        countExpression = propExpression.ToCountExpression<TestDummyClass>();
        Assert.NotNull(
            countExpression,
            $"ToCountExpression<int> returns a valid expression");
        countExpression = propExpression.ToCountExpression<TestDummyClass>();
        Assert.NotNull(
            countExpression,
            $"ToCountExpression<int> returns a valid expression");
    }

    [Test]
    public void TestQueryOrderByArray()
    {
        var source = new TestDummyClass[]
        {
            new()
            {
                IntValue = 4,
                ArrayOfIntegers = new []{ 1, 2, 3, 4 }
            },
            
            new()
            {
                IntValue = 3,
                ArrayOfIntegers = new []{ 1, 2, 3 }
            },
            
            new()
            {
                IntValue = 2,
                ArrayOfIntegers = new []{ 1, 2 }
            },
            
            new() { IntValue = 1 },
            new() { IntValue = 0 },
        };

        var orderedByArray = source.AsQueryable().OrderBy("ArrayOfIntegers").ToList();

        var lastArrayLength = -1;
        orderedByArray.ForEach(item =>
        {
            Assert.True(item.ArrayOfIntegers.Length >= lastArrayLength);
            lastArrayLength = item.ArrayOfIntegers.Length;
        });
    }

    [Test]
    public void TestQueryOrderByArrayDescending()
    {
        var source = new TestDummyClass[]
        {
            new()
            {
                IntValue = 4,
                ArrayOfIntegers = new []{ 1, 2, 3, 4 }
            },
            
            new()
            {
                IntValue = 3,
                ArrayOfIntegers = new []{ 1, 2, 3 }
            },
            
            new()
            {
                IntValue = 2,
                ArrayOfIntegers = new []{ 1, 2 }
            },
            
            new() { IntValue = 1 },
            new() { IntValue = 0 },
        };

        var orderedByArray = source.AsQueryable().OrderByDescending("ArrayOfIntegers").ToList();

        var lastArrayLength = int.MaxValue;
        orderedByArray.ForEach(item =>
        {
            Assert.True(item.ArrayOfIntegers.Length <= lastArrayLength);
            lastArrayLength = item.ArrayOfIntegers.Length;
        });
    }
}
