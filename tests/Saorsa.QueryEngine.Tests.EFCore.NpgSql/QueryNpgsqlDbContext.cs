using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Saorsa.QueryEngine.Tests.EFCore.NpgSql;


/// <summary>
/// Extension of the query db context, used for testing NpgSql drivers.
/// </summary>
public class QueryNpgsqlDbContext : QueryDbContext
{
    /// <summary>
    /// Gets the demo level environment names.
    /// </summary>
    public static readonly string[] DemoEnvironments =
    {
        "Demo",
        "Development"
    };

    /// <summary>
    /// Initializes a new instance of the DbContext class.
    /// </summary>
    /// <param name="logger">Logger instance for the context. Defaults to Serilog console logger if not set.</param>
    public QueryNpgsqlDbContext(ILogger? logger = null) : base(logger)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        Logger.LogInformation($"{GetType().Name} - {nameof(OnConfiguring)} - starting...");
        var connectionString = DemoEnvironments.Contains(EnvironmentName)
            ? "Host=localhost;Port=5951;Database=demodb;Username=demo;Password=demo"
            : "Host=localhost;Port=5950;Database=testdb;Username=test;Password=test";
        Logger.LogDebug($"{GetType().Name} - {nameof(OnConfiguring)} - connection string - {connectionString}");
        optionsBuilder
            .UseNpgsql(connectionString);
        Logger.LogInformation($"{GetType().Name} - {nameof(OnConfiguring)} - complete");
    }
}
