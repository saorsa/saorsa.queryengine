using Saorsa.QueryEngine.Annotations;

namespace Saorsa.QueryEngine.Tests.EFCore.Entities;

[QueryEngineCompile]
public class Department : TestEntityBase
{
    public int Id { get; set; }
    public string Name { get; set; } = $"category-{Guid.NewGuid():N}";
    

    // many-to-one
    public ICollection<User> Users { get; } = new List<User>();
    
    // many-to-one
    public ICollection<Tag> Groups { get; } = new List<Tag>();
    
    // many-to-one
    public ICollection<Department> ChildCategories { get; } = new List<Department>();
    
    // one to many
    public int? ParentCategoryId { get; set; }
    public Department? ParentCategory { get; set; }
}
