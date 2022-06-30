using Saorsa.QueryEngine.Annotations;

namespace Saorsa.QueryEngine.Tests.EFCore.Entities;

[QueryEngineCompile]
public class User : EntityBase
{
    public Guid Id { get; set; }
    public string Username { get; set; } = $"user-{Guid.NewGuid():N}";
    public string? Password { get; set; }
    public string? Gender { get; set; }
    public int? Age { get; set; }
    public long? ExternalId { get; set; }
    
    public UserLogonType? LatestLogonType { get; set; }
    
    // one to many
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    // many-to-many
    public ICollection<Group> Groups { get; } = new List<Group>();
}
