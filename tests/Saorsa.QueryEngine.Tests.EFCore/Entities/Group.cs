using Saorsa.QueryEngine.Annotations;

namespace Saorsa.QueryEngine.Tests.EFCore.Entities;

[QueryEngineCompile]
public class Group : EntityBase
{
    public string Id { get; set; } = $"group-{Guid.NewGuid():N}";
    
    // one to many
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    
    // many-to-many
    public ICollection<User> Users { get; } = new List<User>();
}
