using Microsoft.EntityFrameworkCore;
using Saorsa.QueryEngine.Tests.EFCore;

namespace Saorsa.QueryEngine.Tests.NpgSql.Data;

public class QueryNpgsqlDbContext : QueryDbContextBase
{
    public static readonly string[] DemoEnvironments =
    {
        "Demo",
        "Development"
    };
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (environment != null && DemoEnvironments.Contains(environment))
        {
            optionsBuilder
                .LogTo(Console.WriteLine)
                .UseNpgsql("Host=localhost;Port=5951;Database=demodb;Username=demo;Password=demo");
        }
        else
        {
            optionsBuilder
                .LogTo(Console.WriteLine)
                .UseNpgsql("Host=localhost;Port=5950;Database=testdb;Username=test;Password=test");
        }
    }
}
