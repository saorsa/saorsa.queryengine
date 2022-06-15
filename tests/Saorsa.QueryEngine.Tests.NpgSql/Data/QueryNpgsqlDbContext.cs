using Microsoft.EntityFrameworkCore;
using Saorsa.QueryEngine.Tests.EFCore;

namespace Saorsa.QueryEngine.Tests.NpgSql.Data;

public class QueryNpgsqlDbContext : QueryDbContextBase
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .LogTo(Console.WriteLine)
            .UseNpgsql("Host=localhost;Port=5950;Database=testdb;Username=test;Password=test");
}
