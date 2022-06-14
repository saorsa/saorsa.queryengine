using Microsoft.EntityFrameworkCore;

namespace Saorsa.QueryEngine.Tests.NpgSql.Data;

public class QueryDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Category> Categories => Set<Category>();
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .LogTo(Console.WriteLine)
            .UseNpgsql("Host=localhost;Port=5950;Database=testdb;Username=test;Password=test");
}
