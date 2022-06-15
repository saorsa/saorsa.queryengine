using Saorsa.QueryEngine.Annotations;

namespace Saorsa.QueryEngine.Tests.EFCore.Entities;

[QueryEngineCompile]
public class Category : EntityBase
{
    public int Id { get; set; }
    public string Name { get; set; } = $"category-{Guid.NewGuid():N}";
    

    // many-to-one
    public ICollection<User> Users { get; } = new List<User>();
    
    // many-to-one
    public ICollection<Group> Groups { get; } = new List<Group>();
    
    // many-to-one
    public ICollection<Category> ChildCategories { get; } = new List<Category>();
    
    // one to many
    public int? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }
}
