using NUnit.Framework;
using Saorsa.QueryEngine.Model;

namespace Saorsa.QueryEngine.Tests.EFCore.Common;


public class QueryDbDepartmentTests: EFCoreTestBase
{
    [Test]
    public void TestSimpleInsertDelete()
    {
        var key = Guid.NewGuid().ToString("N");
        var department = new Department
        {
            Name = $"Department-{key}"
        };
        
        var db = GetQueryDbContext();
        db.Departments.AddRange(department);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        
        db.Departments.Remove(department);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        db.Dispose();
    }
    
    [Test]
    public void TestSimpleInsertEqualsQueryDelete()
    {
        var key = Guid.NewGuid().ToString("N");
        var department = new Department
        {
            Name = $"Department-{key}"
        };
        
        var db = GetQueryDbContext();
        db.Departments.AddRange(department);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        
        var query = db.Departments
            .Where(new FilterPropertyDescriptor
            {
                Name = nameof(Department.Name),
                FilterType = FilterOperatorType.EqualTo,
                Arguments = new object[] { department.Name }
            });
        
        var queryResults = query.ToList();
        
        Assert.That(queryResults, Is.Not.Empty);
        Assert.That(queryResults.Count, Is.EqualTo(1));
        
        db.Departments.Remove(department);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        db.Dispose();
    }
    
    [Test]
    public void TestQueryIsNullForReferenceColumn()
    {
        var key = Guid.NewGuid().ToString("N");
        var department = new Department
        {
            Name = $"Department-{key}"
        };
        
        var db = GetQueryDbContext();
        db.Departments.AddRange(department);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        
        var query = db.Departments
            .Where(new FilterPropertyDescriptor
            {
                Name = nameof(Department.Name),
                FilterType = FilterOperatorType.EqualTo,
                Arguments = new object[] { department.Name }
            })
            .Where(new FilterPropertyDescriptor
            {
                Name = nameof(Department.ParentDepartment),
                FilterType = FilterOperatorType.IsNull,
            });
        
        var queryResults = query.ToList();
        
        Assert.That(queryResults, Is.Not.Empty);
        Assert.That(queryResults.Count, Is.EqualTo(1));
        
        db.Departments.Remove(department);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        db.Dispose();
    }
    
    [Test]
    public void TestQueryIsNullForReferenceColumnNegative()
    {
        var key = Guid.NewGuid().ToString("N");
        var department = new Department
        {
            Name = $"Department-{key}",
            ParentDepartment = new Department()
        };

        var db = GetQueryDbContext();
        db.Departments.AddRange(department);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(2));
        
        var query = db.Departments
            .Where(new FilterPropertyDescriptor
            {
                Name = nameof(Department.Name),
                FilterType = FilterOperatorType.EqualTo,
                Arguments = new object[] { department.Name }
            })
            .Where(new FilterPropertyDescriptor
            {
                Name = nameof(Department.ParentDepartment),
                FilterType = FilterOperatorType.IsNull,
            });
        
        var queryResults = query.ToList();
        
        Assert.That(queryResults, Is.Empty);
        
        db.Departments.Remove(department);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        db.Dispose();
    }
    
    [Test]
    public void TestQueryIsNotNullForReferenceColumn()
    {
        var key = Guid.NewGuid().ToString("N");
        var parentKey = Guid.NewGuid().ToString("N");
        var department = new Department
        {
            Name = $"Department-{key}",
            ParentDepartment = new Department
            {
                Name = $"Department-{parentKey}"
            }
        };

        var db = GetQueryDbContext();
        db.Departments.AddRange(department);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(2));
        
        var query = db.Departments
            .Where(new FilterPropertyDescriptor
            {
                Name = nameof(Department.Name),
                FilterType = FilterOperatorType.EqualTo,
                Arguments = new object[] { department.Name }
            })
            .Where(new FilterPropertyDescriptor
            {
                Name = nameof(Department.ParentDepartment),
                FilterType = FilterOperatorType.IsNotNull,
            });
        
        var queryResults = query.ToList();
        
        Assert.That(queryResults, Is.Not.Empty);
        Assert.That(queryResults.Count, Is.EqualTo(1));
        Assert.That(queryResults.First().Name, Is.EqualTo(department.Name));

        db.Departments.Remove(department);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        db.Dispose();
    }
    
    [Test]
    public void TestQueryIsNotNullForReferenceColumnNegative()
    {
        var key = Guid.NewGuid().ToString("N");
        var parentKey = Guid.NewGuid().ToString("N");
        var department = new Department
        {
            Name = $"Department-{key}",
            ParentDepartment = new Department
            {
                Name = $"Department-{parentKey}"
            }
        };

        var db = GetQueryDbContext();
        db.Departments.AddRange(department);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(2));
        
        var query = db.Departments
            .Where(new FilterPropertyDescriptor
            {
                Name = nameof(Department.Name),
                FilterType = FilterOperatorType.EqualTo,
                Arguments = new object[] { department.Name }
            })
            .Where(new FilterPropertyDescriptor
            {
                Name = nameof(Department.ParentDepartment),
                FilterType = FilterOperatorType.IsNull,
            });
        
        var queryResults = query.ToList();
        
        Assert.That(queryResults, Is.Empty);

        db.Departments.Remove(department);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(1));
        db.Dispose();
    }
}
