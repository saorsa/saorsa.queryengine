using System.Reflection;

namespace Saorsa.QueryEngine.Tests;

public class QueryEngineTypeSystemTests
{
    [Test]
    public void TestSimpleTypesMapsIntegrity()
    {
        QueryEngine.SimpleTypes.ToList().ForEach(type => {
            Assert.Multiple(() =>
            {
                Assert.That(QueryEngine.SimpleTypesStringMap.ContainsKey(type), Is.True,
                    $"Type '{type}' must be registered in the {nameof(QueryEngine.SimpleTypesStringMap)} map.");
                Assert.That(QueryEngine.SimpleTypesFiltersMap.ContainsKey(type), Is.True,
                    $"Type '{type}' must be registered in the {nameof(QueryEngine.SimpleTypesFiltersMap)} map.");
                Assert.That(QueryEngine.SimpleTypes.Contains(type), Is.True,
                    $"Type '{type}' must be available via the {nameof(QueryEngine.SimpleTypes)} enumeration.");
            });
        });
    }

    [Test]
    public void TestSimpleTypesIntegrity()
    {
        QueryEngine.SimpleTypes.ToList().ForEach(type => {
            Assert.Multiple(() =>
            {
                Assert.That(QueryEngine.IsSimpleType(type));
                Assert.That(type.IsQueryEngineSimpleType(), Is.True);
            });
        });
    }
    
    [TestCase(42)]
    [TestCase(short.MinValue)]
    [TestCase(ushort.MinValue)]
    [TestCase(byte.MinValue)]
    [TestCase(sbyte.MinValue)]
    [TestCase(true)]
    [TestCase('c')]
    [TestCase("string")]
    [TestCase(1.2d)]
    [TestCase(1u)]
    [TestCase(1.12f)]
    [TestCase(1L)]
    [TestCase(1UL)]
    [TestCase(1.0121)]
    [TestCase(0x1)]
    [TestCase(0x00000012)]
    public void TestQueryEngineSimpleTypeObjectCase(object candidate)
    {
        var type = candidate.GetType();
        Assert.That(type, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(QueryEngine.IsSimpleType(type),
                $"Type '{type}' is a simple type.");
            Assert.That(type.IsQueryEngineSimpleType(), Is.True,
                $"Type '{type}' is a simple type.");
        });
    }

    [Test]
    public void TestQueryEngineSimpleTypeStructureObjects()
    {
        new List<object>
        {
            decimal.MinusOne,
            DateTime.Now,
            DateOnly.MinValue,
            TimeSpan.Zero,
            TimeOnly.MinValue,
            DateTimeOffset.Now,
        }.ForEach(TestQueryEngineSimpleTypeObjectCase);
    }

    [Test]
    public void TestStringRepresentationOnEnums()
    {
        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsEnum && t.IsPublic)
            .ToList()
            .ForEach(enumType =>
            {
                var stringKey = QueryEngine.GetStringRepresentation(enumType);
                Assert.That(stringKey, Is.EqualTo(QueryEngine.SpecialTypeStringKeys.Enumeration),
                    $"Type '{enumType}' is enumeration and its string representation must be " +
                    $"'{QueryEngine.SpecialTypeStringKeys.Enumeration}'.");
            });
    }
    
    [Test]
    public void TestStringRepresentationOnSimpleTypes()
    {
       QueryEngine.SimpleTypes
            .ToList()
            .ForEach(simpleType =>
            {
                var stringKey = QueryEngine.GetStringRepresentation(simpleType);
                Assert.That(stringKey, Is.EqualTo(QueryEngine.SimpleTypesStringMap[simpleType]));
            });
    }
}
