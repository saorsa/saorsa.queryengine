namespace Saorsa.QueryEngine.Tests.EFCore;


/// <summary>
/// Simple base DbContext for demonstration purposes of Query Engine against an EFCore persistence store.
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


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(e => e.Tags)
            .WithMany(e => e.Users)
            .UsingEntity(
                "TagUsers",
                l => l.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagId").HasPrincipalKey(nameof(Tag.Id)),
                r => r.HasOne(typeof(User)).WithMany().HasForeignKey("UserId").HasPrincipalKey(nameof(User.Id)),
                j => j.HasKey("TagId", "UserId"));
    }
}
