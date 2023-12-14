using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Saorsa.QueryEngine.Tests.EFCore.NpgSql;

[SetUpFixture]
public class NpgsqlBootstrapper : EFCoreBootstrapper
{
    /// <summary>
    /// Utility function, gets an instance of the underlying QueryDbContext.
    /// </summary>
    protected override QueryDbContext GetQueryDbContext()
    {
        return new QueryNpgsqlDbContext();
    }

    /// <summary>
    /// Ensures the state of the DbContext and its persistence mode.
    /// </summary>
    /// <param name="dbContext">Target db context.</param>
    protected override async Task InitializeDbAsync(DbContext dbContext)
    {
        Assert.That(dbContext.Database.CanConnect, Is.True, 
            $"Can connect to database for context [{dbContext.GetType().Name}]");

        Logger.LogInformation($"{dbContext.GetType().Name} - Starting database migration.");
        await dbContext.Database.MigrateAsync();
        Logger.LogInformation($"{dbContext.GetType().Name} - Database migration complete.");
    }
}
