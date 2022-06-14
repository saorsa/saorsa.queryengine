namespace Saorsa.QueryEngine.Tests.TestTypes;

public class Group
{
    public string Id { get; set; } = $"group-{Guid.NewGuid():N}";
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
    
    // one to many
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    
    // many-to-many
    public ICollection<User> Users { get; } = new List<User>();
}
