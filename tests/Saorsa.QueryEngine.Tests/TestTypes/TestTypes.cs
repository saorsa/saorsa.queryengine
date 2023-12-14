// ReSharper disable ClassNeverInstantiated.Global
namespace Saorsa.QueryEngine.Tests.TestTypes;

public enum TestEnum
{
    One, Two, Three
}

public enum TestEnumNoMembers
{
}

public struct TestStructure
{
    public int IntValue { get; set; }
    public byte ByteValue { get; set; }
}

[QueryEngineIgnore]
public class TestIgnoreClass
{
    [QueryEngineIgnore]
    public int IgnoredIntValue { get; set; }
    
    [QueryEngineIgnore]
    public float? IgnoredFloatValue { get; set; }
}

[QueryEngineIgnore]
public struct TestIgnoreStructure
{
}

public class TestNonIgnoreClassWithIgnoreProperties
{
    public int? IntValue { get; set; }
    
    [QueryEngineIgnore]
    public string? IgnoredStringValue { get; set; }
}

public class TestRecursiveClass
{
    public TestRecursiveClass? RecursiveProperty { get; set; }
}

[QueryEngineIgnore]
public enum TestIgnoreEnum
{
}


public class TestDummyClass
{
    [QueryEngineIgnore]
    public object? IgnoreValue { get; set; }

    [QueryEngineIgnore]
    public TestDummyClass? NestedDummy { get; set; }

    public int IntValue { get; set; }
        
    public long? LongValue { get; set; }
        
    public DateTime? DateTimeValue { get; set; }

    public string StringValue { get; set; } = default!;
        
    public string? OptionalStringValue { get; set; }
        
    public TestEnum EnumValue { get; set; }

    public int[] ArrayOfIntegers { get; set; } = Array.Empty<int>();

    public IEnumerable<int>? EnumerableOfIntegers { get; set; }
}
