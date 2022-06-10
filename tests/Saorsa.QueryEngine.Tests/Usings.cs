global using NUnit.Framework;
using Saorsa.QueryEngine.Annotations;

public enum TestDummyEnum
{
    One, Two, Three
}

public struct TestDummyStructure
{
    public int X { get; set; }
}

[QueryEngineIgnore]
public class TestDummyIgnoredClass
{
}

public class TestDummyClass
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
