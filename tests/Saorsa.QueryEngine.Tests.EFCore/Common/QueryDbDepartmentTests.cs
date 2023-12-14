using NUnit.Framework;
using Saorsa.QueryEngine.Model;

namespace Saorsa.QueryEngine.Tests.EFCore.Common;


public class QueryDbDepartmentTests: TestBase
{
    [Test]
    public void TestSimpleInsertDelete()
    {
        var key = Guid.NewGuid().ToString("N");
        var category = new Department
        {
            Name = $"Group-{key}"
        };
        
        var db = GetQueryDbContext();
        db.Departments.AddRange(category);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        
        db.Departments.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        db.Dispose();
    }
    
    [Test]
    public void TestSimpleInsertEqualsQueryDelete()
    {
        var key = Guid.NewGuid().ToString("N");
        var category = new Department
        {
            Name = $"Group-{key}"
        };
        
        var db = GetQueryDbContext();
        db.Departments.AddRange(category);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        
        var query = db.Departments
            .Where(new PropertyFilter
            {
                Name = nameof(Department.Name),
                FilterType = FilterType.EQ,
                Arguments = new object[] { category.Name }
            });
        
        var queryResults = query.ToList();
        
        Assert.That(queryResults, Is.Not.Empty);
        Assert.That(queryResults.Count, Is.EqualTo(1));
        
        db.Departments.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        db.Dispose();
    }
    
    [Test]
    public void TestQueryIsNullForReferenceColumn()
    {
        var key = Guid.NewGuid().ToString("N");
        var category = new Department
        {
            Name = $"Group-{key}"
        };
        
        var db = GetQueryDbContext();
        db.Departments.AddRange(category);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        
        var query = db.Departments
            .Where(new PropertyFilter
            {
                Name = nameof(Department.Name),
                FilterType = FilterType.EQ,
                Arguments = new object[] { category.Name }
            })
            .Where(new PropertyFilter
            {
                Name = nameof(Department.ParentDepartment),
                FilterType = FilterType.IS_NULL,
            });
        
        var queryResults = query.ToList();
        
        Assert.That(queryResults, Is.Not.Empty);
        Assert.That(queryResults.Count, Is.EqualTo(1));
        
        db.Departments.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        db.Dispose();
    }
    
    [Test]
    public void TestQueryIsNullForReferenceColumnNegative()
    {
        var key = Guid.NewGuid().ToString("N");
        var category = new Department
        {
            Name = $"Group-{key}",
            ParentDepartment = new Department()
        };

        var db = GetQueryDbContext();
        db.Departments.AddRange(category);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(2));
        
        var query = db.Departments
            .Where(new PropertyFilter
            {
                Name = nameof(Department.Name),
                FilterType = FilterType.EQ,
                Arguments = new object[] { category.Name }
            })
            .Where(new PropertyFilter
            {
                Name = nameof(Department.ParentDepartment),
                FilterType = FilterType.IS_NULL,
            });
        
        var queryResults = query.ToList();
        
        Assert.That(queryResults, Is.Empty);
        
        db.Departments.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        db.Dispose();
    }
    
    [Test]
    public void TestQueryIsNotNullForReferenceColumn()
    {
        var key = Guid.NewGuid().ToString("N");
        var parentKey = Guid.NewGuid().ToString("N");
        var category = new Department
        {
            Name = $"Group-{key}",
            ParentDepartment = new Department
            {
                Name = $"Group-{parentKey}"
            }
        };

        var db = GetQueryDbContext();
        db.Departments.AddRange(category);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(2));
        
        var query = db.Departments
            .Where(new PropertyFilter
            {
                Name = nameof(Department.Name),
                FilterType = FilterType.EQ,
                Arguments = new object[] { category.Name }
            })
            .Where(new PropertyFilter
            {
                Name = nameof(Department.ParentDepartment),
                FilterType = FilterType.IS_NOT_NULL,
            });
        
        var queryResults = query.ToList();
        
        Assert.That(queryResults, Is.Not.Empty);
        Assert.That(queryResults.Count, Is.EqualTo(1));
        Assert.That(queryResults.First().Name, Is.EqualTo(category.Name));

        db.Departments.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        db.Dispose();
    }
    
    [Test]
    public void TestQueryIsNotNullForReferenceColumnNegative()
    {
        var key = Guid.NewGuid().ToString("N");
        var parentKey = Guid.NewGuid().ToString("N");
        var category = new Department
        {
            Name = $"Group-{key}",
            ParentDepartment = new Department
            {
                Name = $"Group-{parentKey}"
            }
        };

        var db = GetQueryDbContext();
        db.Departments.AddRange(category);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(2));
        
        var query = db.Departments
            .Where(new PropertyFilter
            {
                Name = nameof(Department.Name),
                FilterType = FilterType.EQ,
                Arguments = new object[] { category.Name }
            })
            .Where(new PropertyFilter
            {
                Name = nameof(Department.ParentDepartment),
                FilterType = FilterType.IS_NULL,
            });
        
        var queryResults = query.ToList();
        
        Assert.That(queryResults, Is.Empty);

        db.Departments.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        db.Dispose();
    }
}
