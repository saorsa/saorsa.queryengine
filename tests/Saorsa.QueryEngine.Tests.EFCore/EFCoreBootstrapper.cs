using NUnit.Framework;

namespace Saorsa.QueryEngine.Tests.EFCore;


/// <summary>
/// Gets a bootstrapper for all tests in the namespace.
/// </summary>
[SetUpFixture]
public class EFCoreBootstrapper : TestBootstrapper
{
    [OneTimeSetUp]
    public override async Task TestsSetupAsync()
    {
        await base.TestsSetupAsync();
        
        var dbContext = GetQueryDbContext();

        await InitializeDbAsync(dbContext);
        
        await QueryDbPurgeAsync(dbContext);

        await QueryDbEnsureResetCountsAsync(dbContext);
        
        dbContext.Dispose();
    }

    /// <summary>
    /// Bootstrap one-time tear down.
    /// </summary>
    [OneTimeTearDown]
    public virtual async Task TearDownTestsAsync()
    {
        var dbContext = GetQueryDbContext();

        await QueryDbPurgeAsync(dbContext);

        await QueryDbEnsureResetCountsAsync(dbContext);
        
        dbContext.Dispose();

        await base.TestsTearDownAsync();
    }


    /// <summary>
    /// Utility function, gets an instance of the underlying QueryDbContext.
    /// </summary>
    protected virtual QueryDbContext GetQueryDbContext()
    {
        return new QueryDbContext();
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
        var allUsers = dbContext.Users.ToList();
        dbContext.RemoveRange(allUsers);
        var allTags = dbContext.Tags.ToList();
        dbContext.RemoveRange(allTags);
        var allDepartments = dbContext.Departments.ToList();
        dbContext.RemoveRange(allDepartments);
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
