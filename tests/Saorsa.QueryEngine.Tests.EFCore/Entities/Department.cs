using System.ComponentModel.DataAnnotations;
using Saorsa.QueryEngine.Annotations;

namespace Saorsa.QueryEngine.Tests.EFCore.Entities;


/// <summary>
/// Represents a department entity; Departments are hierarchical to make things a bit more complicated.
/// </summary>
[QueryEngineCompile]
public class Department : EntityBase<int>
{
    /// <summary>
    /// Gets or sets the name of the department.
    /// </summary>
    [MaxLength(128)]
    public string Name { get; set; } = $"Department-{Guid.NewGuid():N}";

    /// <summary>
    /// Gets or sets the collection of users assigned to this department.
    /// One department -to- many users relationship.
    /// </summary>
    public ICollection<User> Users { get; } = new List<User>();

    /// <summary>
    /// Gets or sets the collection of tags managed by this department.
    /// One department -to- many tags relationship.
    /// </summary>
    public ICollection<Tag> OwnedTags { get; } = new List<Tag>();

    /// <summary>
    /// Gets or sets the collection of child departments.
    /// Many child departments -to- one department relationship.
    /// </summary>
    public ICollection<Department> ChildDepartments { get; } = new List<Department>();

    /// <summary>
    /// Gets or sets the FK id of the parent department, if any.
    /// One parent department -to- many child departments relationship.
    /// </summary>
    public int? ParentDepartmentId { get; set; }    

    /// <summary>
    /// Gets or sets the FK reference of the parent department, if any.
    /// One parent department -to- many child departments relationship.
    /// </summary>
    public Department? ParentDepartment { get; set; }
}
