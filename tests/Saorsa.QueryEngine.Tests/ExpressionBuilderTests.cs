using System.Reflection;

namespace Saorsa.QueryEngine.Tests;


/// <summary>
/// Tests related to <seealso cref="T:Saorsa.QueryEngine.ExpressionBuilder"/> utility class.
/// </summary>
public class ExpressionBuilderTests
{
    [Test]
    public void TestCreateParameterPropertyWithTypeParam()
    {
        var stringPropExpression = ExpressionBuilder.CreateParameterPropertyExpression<TestDummyClass>(
            nameof(TestDummyClass.IntValue));
        Assert.NotNull(stringPropExpression,
            $"CreateParameterPropertyExpression creates a member expression for property " +
            $"[{nameof(TestDummyClass.IntValue)}]");

        var int32PropExpression = ExpressionBuilder.CreateParameterPropertyExpression<TestDummyClass>(
            nameof(TestDummyClass.StringValue));
        Assert.NotNull(int32PropExpression,
            $"CreateParameterPropertyExpression creates a member expression for property " +
            $"[{nameof(TestDummyClass.StringValue)}]");
    }

    [Test]
    public void TestCreateParameterProperty()
    {
        var stringPropExpression = ExpressionBuilder.CreateParameterPropertyExpression(
            typeof(TestDummyClass),
            nameof(TestDummyClass.StringValue));
        Assert.NotNull(stringPropExpression,
            $"CreateParameterPropertyExpression creates a member expression for property " +
            $"[{nameof(TestDummyClass.StringValue)}]");

        var int32PropExpression = ExpressionBuilder.CreateParameterPropertyExpression<TestDummyClass>(
            nameof(TestDummyClass.IntValue));
        Assert.NotNull(int32PropExpression,
            $"CreateParameterPropertyExpression creates a member expression for property " +
            $"[{nameof(TestDummyClass.IntValue)}]");
    }

    [Test]
    public void TestCreateParameterPropertyWithTypeParamUsingReflection()
    {
        var paramType = typeof(TestDummyClass);
        var instancePublic = BindingFlags.Public | BindingFlags.Instance;

        paramType.GetProperties(instancePublic)
            .ToList()
            .ForEach(mi =>
            {
                var memberExpression = ExpressionBuilder.CreateParameterPropertyExpression<TestDummyClass>(mi.Name);
                Assert.NotNull(memberExpression,
                    $"CreateParameterPropertyExpression creates a member expression for property [{mi.Name}]");
            });
    }

    [Test]
    public void TestCreateParameterPropertyUsingReflection()
    {
        var paramType = typeof(TestDummyClass);
        var instancePublic = BindingFlags.Public | BindingFlags.Instance;

        paramType.GetProperties(instancePublic)
            .ToList()
            .ForEach(mi =>
            {
                var memberExpression = ExpressionBuilder.CreateParameterPropertyExpression(paramType, mi.Name);
                Assert.NotNull(memberExpression,
                    $"CreateParameterPropertyExpression creates a member expression for property [{mi.Name}]");
            });
    }
}
