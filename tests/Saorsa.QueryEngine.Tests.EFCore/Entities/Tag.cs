using Saorsa.QueryEngine.Annotations;

namespace Saorsa.QueryEngine.Tests.EFCore.Entities;

[QueryEngineCompile]
public class Tag : TestEntityBase<string>
{
    // one to many
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    /// <summary>
    /// Gets the collection of users assigned to this role.
    /// Many-to-Many relationship.
    /// </summary>
    public ICollection<User> Users { get; } = new List<User>();
}
