using System.Diagnostics;
using NUnit.Framework;
using Saorsa.QueryEngine.Model;

namespace Saorsa.QueryEngine.Tests.EFCore.Common;


public class QueryDbUserTests : EFCoreTestBase
{
    [Test]
    public void TestSimpleInsertDelete()
    {
        var key = Guid.NewGuid().ToString("N");
        var category = new Department
        {
            Name = $"Department-{key}"
        };
        var users = new User[]
        {
            new()
            {
                Username = $"user1-{key}",
                PasswordHash = Guid.NewGuid().ToString("N"),
                ExternalId = Guid.NewGuid().ToString("N"),
                Age = 30,
                Department = category,
            },
            
            new()
            {
                Username = $"user2-{key}",
                PasswordHash = Guid.NewGuid().ToString("N"),
                Age = 32,
                Department = category,
            },
            
            new()
            {
                Username = $"user3-{key}",
                PasswordHash = Guid.NewGuid().ToString("N"),
                Department = category,
            }
        };

        var db = GetQueryDbContext();
        db.Users.AddRange(users);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));
        
        db.Users.RemoveRange(users);
        db.Departments.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));
        db.Dispose();
    }
    
    [Test]
    public void TestSimpleInsertQueryDelete()
    {
        var key = Guid.NewGuid().ToString("N");
        var category = new Department
        {
            Name = $"Department-{key}"
        };
        var users = new User[]
        {
            new()
            {
                Username = $"user1-{key}",
                PasswordHash = Guid.NewGuid().ToString("N"),
                ExternalId = Guid.NewGuid().ToString("N"),
                Age = 30,
                Department = category,
            },
            
            new()
            {
                Username = $"user2-{key}",
                PasswordHash = Guid.NewGuid().ToString("N"),
                Age = 32,
                Department = category,
            },
            
            new()
            {
                Username = $"user3-{key}",
                PasswordHash = Guid.NewGuid().ToString("N"),
                Department = category,
            }
        };

        var db = GetQueryDbContext();
        db.Users.AddRange(users);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));

        var query = db.Users
            .Where(new FilterPropertyDescriptor
            {
                Name = nameof(User.Age),
                FilterType = FilterOperatorType.LessThan,
                Arguments = new object[] { 40 }
            })
            .Where(new FilterPropertyDescriptor
            {
                Name = nameof(User.DepartmentId),
                FilterType = FilterOperatorType.EqualTo,
                Arguments = new object[] { category.Id }
            });
        
        var queryResults = query.ToList();
        
        Assert.That(
            queryResults.Count,
            Is.EqualTo(users.Count(u => u.Age <= 40)));
        
        
        db.Users.RemoveRange(users);
        db.Departments.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));
        db.Dispose();
    }

    [Test]
    public void TestQueryByNullableColumnForIsNull()
    {
        var key = Guid.NewGuid().ToString("N");
        var category = new Department
        {
            Name = $"Department-{key}"
        };
        var users = new User[]
        {
            new()
            {
                Username = $"user1-{key}",
                PasswordHash = Guid.NewGuid().ToString("N"),
                ExternalId = Guid.NewGuid().ToString("N"),
                Age = 30,
                Department = category,
            },
            
            new()
            {
                Username = $"user2-{key}",
                PasswordHash = Guid.NewGuid().ToString("N"),
                Age = 32,
                Department = category,
            },
            
            new()
            {
                Username = $"user3-{key}",
                PasswordHash = Guid.NewGuid().ToString("N"),
                Department = category,
            }
        };

        var db = GetQueryDbContext();
        db.Users.AddRange(users);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));

        var query = db.Users
            .Where(new FilterPropertyDescriptor
            {
                Name = nameof(User.ExternalId),
                FilterType = FilterOperatorType.IsNull,
            })
            .Where(new FilterPropertyDescriptor
            {
                Name = nameof(User.DepartmentId),
                FilterType = FilterOperatorType.EqualTo,
                Arguments = new object[] { category.Id }
            });
        
        var queryResults = query.ToList();
        
        Assert.That(
            queryResults.Count,
            Is.EqualTo(users.Count(u => u.ExternalId == null)));
        
        
        db.Users.RemoveRange(users);
        db.Departments.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));
        db.Dispose();
    }

    [Test]
    public void TestQueryByNullableColumnForIsNotNull()
    {
        var key = Guid.NewGuid().ToString("N");
        var category = new Department
        {
            Name = $"Department-{key}"
        };
        var users = new User[]
        {
            new()
            {
                Username = $"user1-{key}",
                PasswordHash = Guid.NewGuid().ToString("N"),
                ExternalId = Guid.NewGuid().ToString("N"),
                Age = 30,
                Department = category,
            },
            
            new()
            {
                Username = $"user2-{key}",
                PasswordHash = Guid.NewGuid().ToString("N"),
                Age = 32,
                Department = category,
            },
            
            new()
            {
                Username = $"user3-{key}",
                PasswordHash = Guid.NewGuid().ToString("N"),
                Department = category,
            }
        };

        var db = GetQueryDbContext();
        db.Users.AddRange(users);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));

        var query = db.Users
            .Where(new FilterPropertyDescriptor
            {
                Name = nameof(User.ExternalId),
                FilterType = FilterOperatorType.IsNotNull,
            })
            .Where(new FilterPropertyDescriptor
            {
                Name = nameof(User.DepartmentId),
                FilterType = FilterOperatorType.EqualTo,
                Arguments = new object[] { category.Id }
            });
        
        var queryResults = query.ToList();
        
        Assert.That(
            queryResults.Count,
            Is.EqualTo(users.Count(u => u.ExternalId != null)));
        
        
        db.Users.RemoveRange(users);
        db.Departments.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));
        db.Dispose();
    }
    
    [Test]
    public void TestSimpleInsertQueryEqualToNullDelete()
    {
        var key = Guid.NewGuid().ToString("N");
        var category = new Department
        {
            Name = $"Department-{key}"
        };
        var users = new User[]
        {
            new()
            {
                Username = $"user1-{key}",
                PasswordHash = Guid.NewGuid().ToString("N"),
                ExternalId = Guid.NewGuid().ToString("D"),
                Age = 30,
                Department = category,
            },
            
            new()
            {
                Username = $"user2-{key}",
                PasswordHash = Guid.NewGuid().ToString("N"),
                Age = 32,
                Department = category,
            },
            
            new()
            {
                Username = $"user3-{key}",
                PasswordHash = Guid.NewGuid().ToString("N"),
                Department = category,
            }
        };

        var db = GetQueryDbContext();
        db.Users.AddRange(users);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));

        var query = db.Users
            .Where(new FilterPropertyDescriptor
            {
                Name = nameof(User.ExternalId),
                FilterType = FilterOperatorType.EqualTo,
                Arguments = new object[] { null! }
            })
            .Where(new FilterPropertyDescriptor
            {
                Name = nameof(User.DepartmentId),
                FilterType = FilterOperatorType.EqualTo,
                Arguments = new object[] { category.Id }
            });
        
        var queryResults = query.ToList();
        
        Assert.That(
            queryResults.Count,
            Is.EqualTo(users.Count(u => u.ExternalId == null)));
        
        
        db.Users.RemoveRange(users);
        db.Departments.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));
        db.Dispose();
    }
    
    [Test]
    public void TestSimpleInsertQueryNotEqualToNullDelete()
    {
        var key = Guid.NewGuid().ToString("N");
        var category = new Department
        {
            Name = $"Department-{key}"
        };
        var users = new User[]
        {
            new()
            {
                Username = $"user1-{key}",
                PasswordHash = Guid.NewGuid().ToString("N"),
                ExternalId = Guid.NewGuid().ToString("D"),
                Age = 30,
                Department = category,
            },
            
            new()
            {
                Username = $"user2-{key}",
                PasswordHash = Guid.NewGuid().ToString("N"),
                Age = 32,
                Department = category,
            },
            
            new()
            {
                Username = $"user3-{key}",
                PasswordHash = Guid.NewGuid().ToString("N"),
                Department = category,
            }
        };

        var db = GetQueryDbContext();
        db.Users.AddRange(users);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));

        var query = db.Users
            .Where(new FilterPropertyDescriptor
            {
                Name = nameof(User.ExternalId),
                FilterType = FilterOperatorType.NotEqualTo,
                Arguments = new object[] { null! }
            })
            .Where(new FilterPropertyDescriptor
            {
                Name = nameof(User.DepartmentId),
                FilterType = FilterOperatorType.EqualTo,
                Arguments = new object[] { category.Id }
            });
        
        var queryResults = query.ToList();
        
        Assert.That(
            queryResults.Count,
            Is.EqualTo(users.Count(u => u.ExternalId != null)));

        db.Users.RemoveRange(users);
        db.Departments.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));
        db.Dispose();
    }
    
    
    [Test]
    public void TestSimpleInsertQueryBlockDelete()
    {
        var x = GetQueryDbContext();

        var typeDef = QueryEngine.CompileType<User>();
        var properties = typeDef?.Properties;

        var key = Guid.NewGuid().ToString("N");
        var department = new Department
        {
            Name = $"Department-{key}"
        };
        var externalId = Guid.NewGuid().ToString("D");
        var users = new User[]
        {
            new()
            {
                Username = $"user1-{key}",
                PasswordHash = Guid.NewGuid().ToString("N"),
                ExternalId = externalId,
                Age = 30,
                Department = department,
            },
            
            new()
            {
                Username = $"user2-{key}",
                PasswordHash = Guid.NewGuid().ToString("N"),
                Age = 32,
                Department = department,
            },
            
            new()
            {
                Username = $"user3-{key}",
                PasswordHash = Guid.NewGuid().ToString("N"),
                Department = department,
            }
        };

        var db = GetQueryDbContext();
        db.Users.AddRange(users);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));

        var query = db.Users
            .Where(new FilterBlockDescriptor
            {
                First = new FilterPropertyDescriptor()
                {
                    Name = nameof(User.DepartmentId),
                    FilterType = FilterOperatorType.EqualTo,
                    Arguments = new object[] { department.Id }
                },
                Condition = LogicalOperator.And,
                Others = new []
                {
                    new FilterBlockDescriptor
                    {
                        First = new FilterPropertyDescriptor
                        {
                            Name = nameof(User.ExternalId),
                            FilterType = FilterOperatorType.EqualTo,
                            Arguments = new object[] { externalId }
                        },
                        Condition = LogicalOperator.Or,
                        Others = new []
                        {
                            new FilterBlockDescriptor
                            {
                                First = new FilterPropertyDescriptor
                                {
                                    Name = nameof(User.Age),
                                    FilterType = FilterOperatorType.GreaterThanOrEqual,
                                    Arguments = new object[] { 32 }
                                }
                            }
                        }
                    }
                }
            });
        
        var queryResults = query.ToList();
        
        Assert.That(
            queryResults.Count,
            Is.EqualTo(users.Count(u => u.ExternalId == externalId || u.Age >= 32)));

        query = db.Users
            .Where(new FilterBlockDescriptor
            {
                First = new FilterPropertyDescriptor()
                {
                    Name = $"{nameof(User.Department)}.{nameof(Department.Name)}",
                    FilterType = FilterOperatorType.EqualTo,
                    Arguments = new object[] { department.Name }
                }
            });

        var r = query.ToList();
        
        db.Users.RemoveRange(users);
        db.Departments.Remove(department);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));
        db.Dispose();
    }
}
