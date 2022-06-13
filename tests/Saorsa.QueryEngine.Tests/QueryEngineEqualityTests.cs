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
                EnumerableOfInts = new [] { 1, 2, 3},
            },
            new TestDummyClass
            {
                IntValue = 11,
            },
            new TestDummyClass()
            {
                LongValue = 12,
                StringValue = string.Empty,
                OptionalStringValue = string.Empty
            },
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
        
        
        var propFilter3 = new PropertyFilter
        {
            Name = nameof(TestDummyClass.LongValue).ToCamelCase(),
            FilterType = FilterType.IS_NULL,
        };
        var results3 = QueryEngine.ApplyPropertyFilter(x, propFilter3).ToList();
        Assert.IsNotEmpty(results3);
        
        var propFilter4= new PropertyFilter
        {
            Name = nameof(TestDummyClass.StringValue).ToCamelCase(),
            FilterType = FilterType.IS_NULL,
        };
        var results4 = QueryEngine.ApplyPropertyFilter(x, propFilter4).ToList();
        Assert.IsNotEmpty(results4);
        
        var propFilter5 = new PropertyFilter
        {
            Name = nameof(TestDummyClass.OptionalStringValue).ToCamelCase(),
            FilterType = FilterType.IS_NULL,
        };
        var results5 = QueryEngine.ApplyPropertyFilter(x, propFilter5).ToList();
        Assert.IsNotEmpty(results5);
        
        
        var propFilter6 = new PropertyFilter
        {
            Name = nameof(TestDummyClass.OptionalStringValue).ToCamelCase(),
            FilterType = FilterType.IS_NOT_NULL,
        };
        var results6 = QueryEngine.ApplyPropertyFilter(x, propFilter6).ToList();
        Assert.IsNotEmpty(results6);
        
        var propFilter7 = new PropertyFilter
        {
            Name = nameof(TestDummyClass.StringValue).ToCamelCase(),
            FilterType = FilterType.IS_NOT_NULL,
        };
        var results7 = QueryEngine.ApplyPropertyFilter(x, propFilter7).ToList();
        Assert.IsNotEmpty(results7);
        
        
        var propFilter8 = new PropertyFilter
        {
            Name = nameof(TestDummyClass.LongValue).ToCamelCase(),
            FilterType = FilterType.IS_NOT_NULL,
        };
        var results8 = QueryEngine.ApplyPropertyFilter(x, propFilter8).ToList();
        Assert.IsNotEmpty(results8);
        
        
        var propFilter9 = new PropertyFilter
        {
            Name = nameof(TestDummyClass.ArrayOfInts).ToCamelCase(),
            FilterType = FilterType.IS_NOT_NULL,
        };
        var results9 = QueryEngine.ApplyPropertyFilter(x, propFilter9).ToList();
        Assert.IsNotEmpty(results9);
        
        
        var propFilter10 = new PropertyFilter
        {
            Name = nameof(TestDummyClass.EnumerableOfInts).ToCamelCase(),
            FilterType = FilterType.IS_NULL,
        };
        var results10 = QueryEngine.ApplyPropertyFilter(x, propFilter10).ToList();
        Assert.IsNotEmpty(results10);
    }

    [Test]
    public void TestReferencePropertyQueries()
    {
        var x = new[]
        {
            new TestRecursiveClass
            {
                RecursiveProperty = new TestRecursiveClass()
            },
            new(),
            new(),
        }.AsQueryable();

        var results = QueryEngine.ApplyPropertyFilter(x, new PropertyFilter
        {
            Name = nameof(TestRecursiveClass.RecursiveProperty).ToCamelCase(),
            FilterType = FilterType.IS_NOT_NULL,
        }).ToList();
        
        Assert.IsNotEmpty(results);

        var results2 = QueryEngine.ApplyPropertyFilter(x, new PropertyFilter
        {
            Name = nameof(TestRecursiveClass.RecursiveProperty).ToCamelCase(),
            FilterType = FilterType.IS_NULL,
        }).ToList();
        
        Assert.IsNotEmpty(results2);
    }
}
