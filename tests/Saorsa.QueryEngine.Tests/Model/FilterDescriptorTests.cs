using System.Diagnostics.CodeAnalysis;

namespace Saorsa.QueryEngine.Tests.Model;

public class FilterDescriptorTests
{
    [Test]
    public void TestConstructorFilterTypes()
    {
        Enum.GetValues<FilterType>().ToList().ForEach(filterType => {
            var filter = new FilterDescriptor(filterType);
            Assert.Multiple(() =>
            {
                Assert.That(filter, Is.Not.Null);
                Assert.That(filter.FilterType, Is.EqualTo(filterType));
                Assert.That(filter.Arg1, Is.Null);
                Assert.That(filter.Arg1Required.HasValue, Is.False);
                Assert.That(filter.Arg2, Is.Null);
                Assert.That(filter.Arg2Required.HasValue, Is.False);
            });
        });
    }

    [Test]
    public void TestToString()
    {
        var key1 = Guid.NewGuid().ToString("N");
        var filter1 = new FilterDescriptor(FilterType.EQ, key1, true);
        var toString1 = filter1.ToString();
        Assert.Multiple(() =>
        {
            Assert.That(toString1, Is.Not.Null);
            Assert.That(toString1, Does.Contain(key1));
            Assert.That(filter1.Arg1Required, Is.EqualTo(true));
        });
        
        var key2 = Guid.NewGuid().ToString("N");
        var filter2 = new FilterDescriptor(FilterType.EQ, arg2: key2, arg2Required: false);
        var toString2 = filter2.ToString();
        Assert.Multiple(() =>
        {
            Assert.That(toString2, Is.Not.Null);
            Assert.That(toString2, Does.Contain(key2));
            Assert.That(filter2.Arg2Required, Is.EqualTo(false));
        });
    }

    [TestCase(100)]
    [TestCase(1000)]
    public void TestGetHashCodeCollisions(int sampleSize)
    {
        var sampleIndices = Enumerable.Range(0, sampleSize - 1).ToList();
        var hashes = new Dictionary<int, FilterDescriptor>();
        Enum.GetValues<FilterType>().ToList().ForEach(ft =>
        {
            sampleIndices.ForEach(index =>
            {
                var filterDef = new FilterDescriptor(
                    ft,
                    arg1:  Guid.NewGuid().ToString("N"),
                    arg1Required: index % 2 == 0,
                    arg2: Guid.NewGuid().ToString("N"),
                    arg2Required: index % 2 == 0);

                var hashCode = filterDef.GetHashCode();
                Assert.That(hashes.ContainsKey(hashCode), Is.False);
                hashes.Add(hashCode, filterDef);
            });
        });
    }

    [Test]
    public void TestGetHashCodeCollisionsForCommonFilters()
    {
        var hashes = new Dictionary<int, FilterDescriptor>();
        new []
        {
            FilterDescriptor.IS_NULL,
            FilterDescriptor.IS_NOT_NULL,
            FilterDescriptor.EQ,
            FilterDescriptor.NOT_EQ,
            FilterDescriptor.GT,
            FilterDescriptor.GT_EQ,
            FilterDescriptor.LT,
            FilterDescriptor.LT_EQ,
            FilterDescriptor.RANGE,
            FilterDescriptor.SEQUENCE,
            FilterDescriptor.CONTAINS,
            FilterDescriptor.IS_EMPTY,
            FilterDescriptor.IS_NOT_EMPTY,
        }.ToList().ForEach(f =>
        {
            var hash = f.GetHashCode();
            Assert.That(hashes.ContainsKey(hash), Is.False);
            hashes.Add(hash, f);
        });
    }
    
    [Test]
    [SuppressMessage(
        "Assertion", 
        "NUnit2009:The same value has been provided as both the actual and the expected argument")]
    public void TestEquality()
    {
        var f1 = new FilterDescriptor(FilterType.EQ);
        var f2 = new FilterDescriptor(FilterType.EQ);
        var f3 = new FilterDescriptor(FilterType.NOT_EQ);
        
        Assert.True(f1.Equals(f1));
        Assert.False(f1.Equals(null));
        Assert.Multiple(() =>
        {
            Assert.That(f1, Is.Not.EqualTo(null));
            Assert.That(f1, Is.Not.EqualTo(f3));
            Assert.That(f2, Is.Not.EqualTo(f3));
            Assert.That(f1, Is.EqualTo(f2));
            Assert.That(f1, Is.EqualTo(f1));
        });
    }
    
    [Test]
    [SuppressMessage(
        "Assertion", 
        "NUnit2009:The same value has been provided as both the actual and the expected argument")]
    [SuppressMessage(
        "Assertion", 
        "NUnit2004:Consider using Assert.That(expr, Is.True) instead of Assert.True(expr)")]
    public void TestEqualityToObjects()
    {
        var f1 = new FilterDescriptor(FilterType.EQ);
        var f2 = new FilterDescriptor(FilterType.EQ);
        
        Assert.True(f1.Equals((object)f1));
        Assert.True(f1.Equals((object)f2));
        Assert.False(f1.Equals((object?)null));
    }
}
