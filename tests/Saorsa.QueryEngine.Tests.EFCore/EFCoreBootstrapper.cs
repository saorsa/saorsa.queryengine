using NUnit.Framework;

namespace Saorsa.QueryEngine.Tests.EFCore;

/// <summary>
/// Gets a bootstrapper for all tests in the namespace.
/// </summary>
[SetUpFixture]
public class EFCoreBootstrapper : EFCoreTestBase
{
    /// <summary>
    /// Bootstrap one-time initializer.
    /// </summary>
    [OneTimeSetUp]
    public virtual async Task SetupTestsAsync()
    {
        var dbContext = GetQueryDbContext();

        await InitializeDbAsync(dbContext);
        
        await QueryDbPurgeAsync(dbContext);

        await QueryDbEnsureResetCountsAsync(dbContext);
        
        dbContext.Dispose();
    }

    /// <summary>
    /// Bootstrap one-time tear down.
    /// </summary>
    [OneTimeSetUp]
    public virtual async Task TearDownTestsAsync()
    {
        var dbContext = GetQueryDbContext();

        await QueryDbPurgeAsync(dbContext);

        await QueryDbEnsureResetCountsAsync(dbContext);
        
        dbContext.Dispose();
    }

    /// <summary>
    /// Ensures the state of the DbContext and its persistence mode.
    /// </summary>
    /// <param name="dbContext">Target db context.</param>
    protected virtual Task InitializeDbAsync(DbContext dbContext)
    {
        Assert.That(dbContext.Database.CanConnect, Is.True, 
            $"Can connect to database for context [{dbContext.GetType().Name}]");

        return Task.CompletedTask;
    }

    /// <summary>
    /// Ensures the underlying db store is cleaned up.
    /// </summary>
    /// <param name="dbContext">Target db context.</param>
    protected virtual async Task QueryDbPurgeAsync(QueryDbContext dbContext)
    {
        dbContext.Users.RemoveRange(dbContext.Users);
        dbContext.Tags.RemoveRange(dbContext.Tags);
        dbContext.Departments.RemoveRange(dbContext.Departments);
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Ensures the underlying db store is cleaned up.
    /// </summary>
    /// <param name="dbContext">Target db context.</param>
    protected virtual async Task QueryDbEnsureResetCountsAsync(QueryDbContext dbContext)
    {
        var usersCount = await dbContext.Users.LongCountAsync();
        var tagsCount = await dbContext.Tags.LongCountAsync();
        var departmentsCount = await dbContext.Departments.LongCountAsync();
        
        Assert.That(usersCount, Is.EqualTo(0));
        Assert.That(tagsCount, Is.EqualTo(0));
        Assert.That(departmentsCount, Is.EqualTo(0));
    }
}
