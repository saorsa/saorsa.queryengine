namespace Saorsa.QueryEngine.Tests;

public class QueryEngineUtilTests
{
    [Test]
    public void TestEnumerableTypes()
    {
        TypeExtensionsTests.EnumerableItems.ToList().ForEach(item =>
        {
            var typeName = item.GetType().FullName;
            Assert.True(QueryEngineUtil.IsEnumerable(typeName!));
        });
    }

    [Test]
    public void TestNonEnumerableTypes()
    {
        TypeExtensionsTests.NonEnumerableItems.ToList().ForEach(item =>
        {
            var typeName = item.GetType().FullName;
            Assert.False(QueryEngineUtil.IsEnumerable(typeName!));    
        });
    }
    
    [Test]
    public void TestTypeArgumentExceptions()
    {
        var typeName = string.Empty;
        Assert.Throws<ArgumentException>(() => QueryEngineUtil.IsEnumerable(typeName));
    }
}
