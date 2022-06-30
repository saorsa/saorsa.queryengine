namespace Saorsa.QueryEngine.Tests.EFCore;

public class QueryDbContextBase : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Category> Categories => Set<Category>();
}
