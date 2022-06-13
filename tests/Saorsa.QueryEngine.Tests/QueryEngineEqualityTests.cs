namespace Saorsa.QueryEngine.Tests;

public class QueryEngineEqualityTests
{
    [Test]
    public void TestEqualsWithIntegerColumn()
    {
        var x = new[]
        {
            new TestDummyClass
            {
                IntValue = 42,
            },
            new TestDummyClass
            {
                IntValue = 11,
            },
            new TestDummyClass(),
            new TestDummyClass()
        }.AsQueryable();

        
        var propFilter = new PropertyFilter
        {
            Name = nameof(TestDummyClass.IntValue).ToCamelCase(),
            FilterType = FilterType.EQ,
            Arguments = new object[] { 42 }
        };
        var results = QueryEngine.ApplyPropertyFilter(x, propFilter).ToList();
        Assert.IsNotEmpty(results);
        
        
        var propFilter2 = new PropertyFilter
        {
            Name = nameof(TestDummyClass.IntValue).ToCamelCase(),
            FilterType = FilterType.NOT_EQ,
            Arguments = new object[] { 42 }
        };
        var results2 = QueryEngine.ApplyPropertyFilter(x, propFilter2).ToList();
        Assert.IsNotEmpty(results2);
    }
}
