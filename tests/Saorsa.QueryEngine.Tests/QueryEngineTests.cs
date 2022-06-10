// ReSharper disable ClassNeverInstantiated.Local
using Saorsa.QueryEngine.Annotations;

namespace Saorsa.QueryEngine.Tests;

public class QueryEngineTests
{
    private enum TestDummyEnum
    {
        One, Two, Three
    }

    private struct TestDummyStructure
    {
        public int X { get; set; }
    }

    [QueryEngineIgnore]
    private class TestDummyIgnoredClass
    {
    }

    private class TestDummyClass
    {
        [QueryEngineIgnore]
        public object? IgnoreValue { get; set; }
        
        public int IntValue { get; set; }
        
        public long? LongValue { get; set; }
        
        public DateTime? DateTimeValue { get; set; }

        public string StringValue { get; set; } = default!;
        
        public string? OptionalStringValue { get; set; }
        
        public TestDummyEnum EnumValue { get; set; }

        public int[] ArrayOfInts { get; set; } = Array.Empty<int>();

        public IEnumerable<int>? EnumerableOfInts { get; set; }
    }

    [Test]
    public void TestBuildPropertyDefinitionsOnIgnoredType()
    {
        var results = QueryEngine.BuildTypeDefinition<TestDummyIgnoredClass>();
        Assert.IsEmpty(results!.Properties);
    }
    
    [Test]
    public void TestBuildPropertyDefinitionsOnEnums()
    {
        var results = QueryEngine.BuildTypeDefinition<TestDummyEnum>();
        Assert.IsEmpty(results!.Properties);
    }

    [Test]
    public void TestBuildPropertyDefinitionsOnSimpleTypes()
    {
        QueryEngine.SimpleTypes.ToList().ForEach(t =>
        {
            var results = QueryEngine.BuildTypeDefinition(t);
            Assert.IsNotNull(results);
            Assert.IsEmpty(results!.Properties,
                $"Expected no property defs for simple type {t.Name}");
        });
    }
    
    [Test]
    public void TestBuildPropertyDefinitionsOnArrayTypes()
    {
        new []
        {
            typeof(int[]),
            typeof(List<string>),
            typeof(Dictionary<string, object>)
        }.ToList().ForEach(t =>
        {
            var results = QueryEngine.BuildTypeDefinition(t);
            Assert.That(results, Is.Empty,
                $"Expected no property defs for arrays and generic enumerations {t.Name}");
        });
    }
    
    [Test]
    public void TestBuildPropertyDefinitionsOnClass()
    {
        var results = QueryEngine.BuildTypeDefinition<TestDummyClass>();
        Assert.IsNotNull(results);
        Assert.IsNotEmpty(results!.Properties);
    }
}
