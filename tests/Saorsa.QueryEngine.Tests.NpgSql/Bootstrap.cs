using Microsoft.EntityFrameworkCore;
using Saorsa.QueryEngine.Tests.NpgSql.Data;

namespace Saorsa.QueryEngine.Tests.NpgSql;

[SetUpFixture]
public class BootstrapTestId
{
    public static readonly Guid TestId = Guid.NewGuid();
    
    [OneTimeSetUp]
    public void SetupDb()
    {
        var dbContext = new QueryNpgsqlDbContext();
        //Assert.That(dbContext.Database.CanConnect, Is.True);
        Assert.DoesNotThrow(() => dbContext.Database.Migrate());
        
        PurgeDb(dbContext);
        EnsureCounts(dbContext);
        
        dbContext.Dispose();
    }

    private void PurgeDb(QueryNpgsqlDbContext npgsqlDb)
    {
        npgsqlDb.Users.RemoveRange(npgsqlDb.Users);
        npgsqlDb.Groups.RemoveRange(npgsqlDb.Groups);
        npgsqlDb.Categories.RemoveRange(npgsqlDb.Categories);
        npgsqlDb.SaveChanges();
    }

    private void EnsureCounts(QueryNpgsqlDbContext npgsqlDb)
    {
        var usersCount = npgsqlDb.Users.LongCount();
        var groupsCount = npgsqlDb.Groups.LongCount();
        var categoriesCount = npgsqlDb.Categories.LongCount();
        
        Assert.That(usersCount, Is.EqualTo(0));
        Assert.That(groupsCount, Is.EqualTo(0));
        Assert.That(categoriesCount, Is.EqualTo(0));
    }
}
