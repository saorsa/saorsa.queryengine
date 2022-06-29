using Saorsa.QueryEngine.Tests.NpgSql.Data;

namespace Saorsa.QueryEngine.Tests.NpgSql.Users;

public class CategoryQueryTests
{
    [Test]
    public void TestSimpleInsertDelete()
    {
        var key = Guid.NewGuid().ToString("N");
        var category = new Category
        {
            Name = $"Group-{key}"
        };
        
        var db = new QueryNpgsqlDbContext();
        db.Categories.AddRange(category);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        
        db.Categories.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        db.Dispose();
    }
    
    [Test]
    public void TestSimpleInsertEqualsQueryDelete()
    {
        var key = Guid.NewGuid().ToString("N");
        var category = new Category
        {
            Name = $"Group-{key}"
        };
        
        var db = new QueryNpgsqlDbContext();
        db.Categories.AddRange(category);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        
        var query = db.Categories
            .Where(new PropertyFilter
            {
                Name = nameof(Category.Name),
                FilterType = FilterType.EQ,
                Arguments = new object[] { category.Name }
            });
        
        var queryResults = query.ToList();
        
        Assert.That(queryResults, Is.Not.Empty);
        Assert.That(queryResults.Count, Is.EqualTo(1));
        
        db.Categories.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        db.Dispose();
    }
    
    [Test]
    public void TestQueryIsNullForReferenceColumn()
    {
        var key = Guid.NewGuid().ToString("N");
        var category = new Category
        {
            Name = $"Group-{key}"
        };
        
        var db = new QueryNpgsqlDbContext();
        db.Categories.AddRange(category);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        
        var query = db.Categories
            .Where(new PropertyFilter
            {
                Name = nameof(Category.Name),
                FilterType = FilterType.EQ,
                Arguments = new object[] { category.Name }
            })
            .Where(new PropertyFilter
            {
                Name = nameof(Category.ParentCategory),
                FilterType = FilterType.IS_NULL,
            });
        
        var queryResults = query.ToList();
        
        Assert.That(queryResults, Is.Not.Empty);
        Assert.That(queryResults.Count, Is.EqualTo(1));
        
        db.Categories.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        db.Dispose();
    }
    
    [Test]
    public void TestQueryIsNullForReferenceColumnNegative()
    {
        var key = Guid.NewGuid().ToString("N");
        var category = new Category
        {
            Name = $"Group-{key}",
            ParentCategory = new Category()
        };
        
        var db = new QueryNpgsqlDbContext();
        db.Categories.AddRange(category);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(2));
        
        var query = db.Categories
            .Where(new PropertyFilter
            {
                Name = nameof(Category.Name),
                FilterType = FilterType.EQ,
                Arguments = new object[] { category.Name }
            })
            .Where(new PropertyFilter
            {
                Name = nameof(Category.ParentCategory),
                FilterType = FilterType.IS_NULL,
            });
        
        var queryResults = query.ToList();
        
        Assert.That(queryResults, Is.Empty);
        
        db.Categories.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        db.Dispose();
    }
    
    [Test]
    public void TestQueryIsNotNullForReferenceColumn()
    {
        var key = Guid.NewGuid().ToString("N");
        var parentKey = Guid.NewGuid().ToString("N");
        var category = new Category
        {
            Name = $"Group-{key}",
            ParentCategory = new Category
            {
                Name = $"Group-{parentKey}"
            }
        };
        
        var db = new QueryNpgsqlDbContext();
        db.Categories.AddRange(category);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(2));
        
        var query = db.Categories
            .Where(new PropertyFilter
            {
                Name = nameof(Category.Name),
                FilterType = FilterType.EQ,
                Arguments = new object[] { category.Name }
            })
            .Where(new PropertyFilter
            {
                Name = nameof(Category.ParentCategory),
                FilterType = FilterType.IS_NOT_NULL,
            });
        
        var queryResults = query.ToList();
        
        Assert.That(queryResults, Is.Not.Empty);
        Assert.That(queryResults.Count, Is.EqualTo(1));
        Assert.That(queryResults.First().Name, Is.EqualTo(category.Name));

        db.Categories.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        db.Dispose();
    }
    
    [Test]
    public void TestQueryIsNotNullForReferenceColumnNegative()
    {
        var key = Guid.NewGuid().ToString("N");
        var parentKey = Guid.NewGuid().ToString("N");
        var category = new Category
        {
            Name = $"Group-{key}",
            ParentCategory = new Category
            {
                Name = $"Group-{parentKey}"
            }
        };
        
        var db = new QueryNpgsqlDbContext();
        db.Categories.AddRange(category);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(2));
        
        var query = db.Categories
            .Where(new PropertyFilter
            {
                Name = nameof(Category.Name),
                FilterType = FilterType.EQ,
                Arguments = new object[] { category.Name }
            })
            .Where(new PropertyFilter
            {
                Name = nameof(Category.ParentCategory),
                FilterType = FilterType.IS_NULL,
            });
        
        var queryResults = query.ToList();
        
        Assert.That(queryResults, Is.Empty);

        db.Categories.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        db.Dispose();
    }
}
