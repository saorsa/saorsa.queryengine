namespace Saorsa.QueryEngine.Tests.TestTypes;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = $"category-{Guid.NewGuid():N}";
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
    

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
