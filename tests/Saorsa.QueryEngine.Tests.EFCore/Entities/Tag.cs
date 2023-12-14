using System.ComponentModel.DataAnnotations;
using Saorsa.QueryEngine.Annotations;

namespace Saorsa.QueryEngine.Tests.EFCore.Entities;


/// <summary>
/// A simple entity representing a tag. The ID of the tag represents the tag itself.
/// </summary>
[QueryEngineCompile]
public class Tag : EntityBase<string>
{
    /// <summary>
    /// Gets or sets the unique ID (PK) of the entity.
    /// </summary>
    [MaxLength(128)]
    public override string Id { get; set; } = default!;

    /// <summary>
    /// Gets the FK id of the owning department. Optional.
    /// One owning department -to- many owned tags.
    /// </summary>
    public int? OwnerDepartmentId { get; set; }

    /// <summary>
    /// Gets the FK reference of the owning department. Optional.
    /// One owning department -to- many owned tags.
    /// </summary>
    public Department? OwnerDepartment { get; set; }

    /// <summary>
    /// Gets the collection of users associated with this tag.
    /// Many-to-Many relationship.
    /// </summary>
    public ICollection<User> Users { get; } = new List<User>();
}
