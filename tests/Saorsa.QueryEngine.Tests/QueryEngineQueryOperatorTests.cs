namespace Saorsa.QueryEngine.Tests;

public class QueryEngineQueryOperatorTests
{
    readonly IQueryable<TestDummyClass> Queryable01 = new[]
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
    
    [Test]
    public void TestEqualsWithIntegerColumn()
    {
        
        var propFilter = new PropertyFilter
        {
            Name = nameof(TestDummyClass.IntValue).ToCamelCase(),
            FilterType = FilterType.EQ,
            Arguments = new object[] { 42 }
        };
        var results = QueryEngine.AddPropertyFilter(Queryable01, propFilter).ToList();
        Assert.IsNotEmpty(results);
        
        
        var propFilter2 = new PropertyFilter
        {
            Name = nameof(TestDummyClass.IntValue).ToCamelCase(),
            FilterType = FilterType.NOT_EQ,
            Arguments = new object[] { 42 }
        };
        var results2 = QueryEngine.AddPropertyFilter(Queryable01, propFilter2).ToList();
        Assert.IsNotEmpty(results2);
        
        
        var propFilter3 = new PropertyFilter
        {
            Name = nameof(TestDummyClass.LongValue).ToCamelCase(),
            FilterType = FilterType.IS_NULL,
        };
        var results3 = QueryEngine.AddPropertyFilter(Queryable01, propFilter3).ToList();
        Assert.IsNotEmpty(results3);
        
        var propFilter4= new PropertyFilter
        {
            Name = nameof(TestDummyClass.StringValue).ToCamelCase(),
            FilterType = FilterType.IS_NULL,
        };
        var results4 = QueryEngine.AddPropertyFilter(Queryable01, propFilter4).ToList();
        Assert.IsNotEmpty(results4);
        
        var propFilter5 = new PropertyFilter
        {
            Name = nameof(TestDummyClass.OptionalStringValue).ToCamelCase(),
            FilterType = FilterType.IS_NULL,
        };
        var results5 = QueryEngine.AddPropertyFilter(Queryable01, propFilter5).ToList();
        Assert.IsNotEmpty(results5);
        
        
        var propFilter6 = new PropertyFilter
        {
            Name = nameof(TestDummyClass.OptionalStringValue).ToCamelCase(),
            FilterType = FilterType.IS_NOT_NULL,
        };
        var results6 = QueryEngine.AddPropertyFilter(Queryable01, propFilter6).ToList();
        Assert.IsNotEmpty(results6);
        
        var propFilter7 = new PropertyFilter
        {
            Name = nameof(TestDummyClass.StringValue).ToCamelCase(),
            FilterType = FilterType.IS_NOT_NULL,
        };
        var results7 = QueryEngine.AddPropertyFilter(Queryable01, propFilter7).ToList();
        Assert.IsNotEmpty(results7);
        
        
        var propFilter8 = new PropertyFilter
        {
            Name = nameof(TestDummyClass.LongValue).ToCamelCase(),
            FilterType = FilterType.IS_NOT_NULL,
        };
        var results8 = QueryEngine.AddPropertyFilter(Queryable01, propFilter8).ToList();
        Assert.IsNotEmpty(results8);
        
        
        var propFilter9 = new PropertyFilter
        {
            Name = nameof(TestDummyClass.ArrayOfInts).ToCamelCase(),
            FilterType = FilterType.IS_NOT_NULL,
        };
        var results9 = QueryEngine.AddPropertyFilter(Queryable01, propFilter9).ToList();
        Assert.IsNotEmpty(results9);
        
        
        var propFilter10 = new PropertyFilter
        {
            Name = nameof(TestDummyClass.EnumerableOfInts).ToCamelCase(),
            FilterType = FilterType.IS_NULL,
        };
        var results10 = QueryEngine.AddPropertyFilter(Queryable01, propFilter10).ToList();
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

        var results = QueryEngine.AddPropertyFilter(x, new PropertyFilter
        {
            Name = nameof(TestRecursiveClass.RecursiveProperty).ToCamelCase(),
            FilterType = FilterType.IS_NOT_NULL,
        }).ToList();
        
        Assert.IsNotEmpty(results);

        var results2 = QueryEngine.AddPropertyFilter(x, new PropertyFilter
        {
            Name = nameof(TestRecursiveClass.RecursiveProperty).ToCamelCase(),
            FilterType = FilterType.IS_NULL,
        }).ToList();
        
        Assert.IsNotEmpty(results2);
    }

    [Test]
    public void TestGreaterThan()
    {
        var results = QueryEngine
            .AddPropertyFilter(Queryable01, new PropertyFilter
            {
                Name = nameof(TestDummyClass.LongValue).ToCamelCase(),
                FilterType = FilterType.GT_EQ,
                Arguments = new object[] { 12 }
            }).ToList();
        
        Assert.IsNotEmpty(results);
    }
    
    [Test]
    public void TestGreaterThan2()
    {
        var results = QueryEngine
            .AddPropertyFilter(Queryable01, new PropertyFilter
            {
                Name = nameof(TestDummyClass.LongValue).ToCamelCase(),
                FilterType = FilterType.GT,
                Arguments = new object[] { 11 }
            });

        var l1 = results.ToList();
        Assert.IsNotNull(l1);


        results = QueryEngine.AddPropertyFilter(results, new PropertyFilter
        {
            Name = nameof(TestDummyClass.LongValue).ToCamelCase(),
            FilterType = FilterType.LT,
            Arguments = new object[] { 12 }
        });
        var l2 = results.ToList();
        
        Assert.IsNotNull(l2);
    }
}
