using Microsoft.Extensions.Logging;

namespace Saorsa.QueryEngine.Tests.EFCore;


/// <summary>
/// Base DbContext for demonstration purposes of Query Engine against an EFCore persistence store.
/// Base implementation uses InMemory persistence store.
/// </summary>
public class QueryDbContext : DbContext
{
    /// <summary>
    /// Gets the Users entity set.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Gets the Tags entity set.
    /// </summary>
    public DbSet<Tag> Tags => Set<Tag>();

    /// <summary>
    /// Gets the Departments entity set.
    /// </summary>
    public DbSet<Department> Departments => Set<Department>();

    /// <summary>
    /// Gets a logger for the context.
    /// </summary>
    protected Microsoft.Extensions.Logging.ILogger Logger { get; }

    /// <summary>
    /// Gets the runtime environment of the DbContext.
    /// </summary>
    protected string EnvironmentName => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Test";

    /// <summary>
    /// Initializes a new instance of the DbContext class.
    /// </summary>
    /// <param name="logger">Logger instance for the context. Defaults to Serilog console logger if not set.</param>
    public QueryDbContext(ILogger? logger = null)
    {
        Logger = logger ?? TestUtils.CreateDefaultLogger<QueryDbContext>();
        Logger.LogDebug($"{GetType().Name} - initialization complete, environment is {EnvironmentName}");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        Logger.LogInformation($"{GetType().Name} - {nameof(OnConfiguring)} - starting...");
        optionsBuilder
            .UseInMemoryDatabase(nameof(QueryDbContext));
        Logger.LogInformation($"{GetType().Name} - {nameof(OnConfiguring)} - complete");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(e => e.Tags)
            .WithMany(e => e.Users)
            .UsingEntity(
                "TagUsers",
                l => l.HasOne(typeof(Tag))
                    .WithMany().HasForeignKey("TagId").HasPrincipalKey(nameof(Tag.Id)),
                r => r.HasOne(typeof(User))
                    .WithMany().HasForeignKey("UserId").HasPrincipalKey(nameof(User.Id)),
                j => j.HasKey("TagId", "UserId"));
    }
}
