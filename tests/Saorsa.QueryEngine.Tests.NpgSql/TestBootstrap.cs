using Saorsa.QueryEngine.Tests.NpgSql.Data;

namespace Saorsa.QueryEngine.Tests.NpgSql;

public class TestBootstrap
{
    public static readonly Guid TestId = Guid.NewGuid();
    
    [OneTimeSetUp]
    public void SetupDb()
    {
        var dbContext = new QueryDbContext();
        Assert.That(dbContext.Database.CanConnect, Is.True);
        
        PurgeDb(dbContext);
        EnsureCounts(dbContext);
        
        dbContext.Dispose();
    }

    private void PurgeDb(QueryDbContext db)
    {
        db.Users.RemoveRange(db.Users);
        db.Groups.RemoveRange(db.Groups);
        db.Categories.RemoveRange(db.Categories);
        db.SaveChanges();
    }

    private void EnsureCounts(QueryDbContext db)
    {
        var usersCount = db.Users.LongCount();
        var groupsCount = db.Groups.LongCount();
        var categoriesCount = db.Categories.LongCount();
        
        Assert.That(usersCount, Is.EqualTo(0));
        Assert.That(groupsCount, Is.EqualTo(0));
        Assert.That(categoriesCount, Is.EqualTo(0));
    }
}
