namespace Saorsa.QueryEngine.Tests.EFCore;


/// <summary>
/// Base class for a tests.
/// </summary>
public class EFCoreTestBase
{
    /// <summary>
    /// Unique ID of the test run, generated once during bootstrap.
    /// </summary>
    protected string TestRun = $"{nameof(Bootstrapper)}-TestRun-{Guid.NewGuid():N}";

    /// <summary>
    /// Utility function, gets an instance of the underlying QueryDbContext.
    /// </summary>
    protected virtual QueryDbContext GetQueryDbContext()
    {
        return new QueryDbContext();
    }
}
