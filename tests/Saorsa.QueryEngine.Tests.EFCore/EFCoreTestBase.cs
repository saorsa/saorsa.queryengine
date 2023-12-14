namespace Saorsa.QueryEngine.Tests.EFCore;


/// <summary>
/// Base class for a tests with support for QueryDbContext.
/// </summary>
public abstract class EFCoreTestBase : TestBase
{
    /// <summary>
    /// Utility function, gets an instance of the underlying QueryDbContext.
    /// </summary>
    protected virtual QueryDbContext GetQueryDbContext()
    {
        return new QueryDbContext();
    }
}
