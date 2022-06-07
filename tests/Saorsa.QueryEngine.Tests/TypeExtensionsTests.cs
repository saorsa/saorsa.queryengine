namespace Saorsa.QueryEngine.Tests;

public class TypeExtensionsTests
{
    public static readonly object[] EnumerableItems = {
        new[] { 1, 2, 3, 4, 5 },
        new[] { Guid.Empty, Guid.NewGuid() },
        new[] { string.Empty },
        new[] { DateTime.Now },
        new object[] { Guid.NewGuid(), DateTime.Now, string.Empty },
        new[] { 1.0f, 2.0f, 17f },
        "strings are enumerable too"
    };

    public static readonly object[] NonEnumerableItems = {
        Guid.NewGuid(),
        DateTimeOffset.UtcNow,
        2f,
        30d,
    };
    
    [Test]
    public void TestEnumerableTypes()
    {
        EnumerableItems.ToList().ForEach(item =>
        {
            var type = item.GetType();
            Assert.True(type.IsEnumerable());
        });
    }
    
    [Test]
    public void TestNonEnumerableTypes()
    {
        NonEnumerableItems.ToList().ForEach(item =>
        {
            var type = item.GetType();
            Assert.False(type.IsEnumerable());    
        });
    }
}
