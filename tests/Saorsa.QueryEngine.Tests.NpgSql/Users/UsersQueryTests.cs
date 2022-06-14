using Saorsa.QueryEngine.Model;
using Saorsa.QueryEngine.Tests.NpgSql.Data;

namespace Saorsa.QueryEngine.Tests.NpgSql.Users;

public class UsersQueryTests
{
    [Test]
    public void TestSimpleInsertDelete()
    {
        var key = Guid.NewGuid().ToString("N");
        var category = new Category
        {
            Name = $"Group-{key}"
        };
        var users = new User[]
        {
            new()
            {
                Username = $"user1-{key}",
                Password = Guid.NewGuid().ToString("N"),
                ExternalId = 123456,
                Age = 30,
                Category = category,
            },
            
            new()
            {
                Username = $"user2-{key}",
                Password = Guid.NewGuid().ToString("N"),
                Age = 32,
                Category = category,
            },
            
            new()
            {
                Username = $"user3-{key}",
                Password = Guid.NewGuid().ToString("N"),
                Category = category,
            }
        };

        var db = new QueryDbContext();
        db.Users.AddRange(users);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));
        
        db.Users.RemoveRange(users);
        db.Categories.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));
        db.Dispose();
    }
    
    [Test]
    public void TestSimpleInsertQueryDelete()
    {
        var key = Guid.NewGuid().ToString("N");
        var category = new Category
        {
            Name = $"Group-{key}"
        };
        var users = new User[]
        {
            new()
            {
                Username = $"user1-{key}",
                Password = Guid.NewGuid().ToString("N"),
                ExternalId = 123456,
                Age = 30,
                Category = category,
            },
            
            new()
            {
                Username = $"user2-{key}",
                Password = Guid.NewGuid().ToString("N"),
                Age = 32,
                Category = category,
            },
            
            new()
            {
                Username = $"user3-{key}",
                Password = Guid.NewGuid().ToString("N"),
                Category = category,
            }
        };

        var db = new QueryDbContext();
        db.Users.AddRange(users);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));

        var query = db.Users
            .AddPropertyFilter(new PropertyFilter
            {
                Name = nameof(User.Age),
                FilterType = FilterType.LT,
                Arguments = new object[] { 40 }
            })
            .AddPropertyFilter(new PropertyFilter
            {
                Name = nameof(User.CategoryId),
                FilterType = FilterType.EQ,
                Arguments = new object[] { category.Id }
            });
        
        var queryResults = query.ToList();
        
        Assert.That(
            queryResults.Count,
            Is.EqualTo(users.Count(u => u.Age <= 40)));
        
        
        db.Users.RemoveRange(users);
        db.Categories.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));
        db.Dispose();
    }

    [Test]
    public void TestQueryByNullableColumnForIsNull()
    {
        var key = Guid.NewGuid().ToString("N");
        var category = new Category
        {
            Name = $"Group-{key}"
        };
        var users = new User[]
        {
            new()
            {
                Username = $"user1-{key}",
                Password = Guid.NewGuid().ToString("N"),
                ExternalId = 123456,
                Age = 30,
                Category = category,
            },
            
            new()
            {
                Username = $"user2-{key}",
                Password = Guid.NewGuid().ToString("N"),
                Age = 32,
                Category = category,
            },
            
            new()
            {
                Username = $"user3-{key}",
                Password = Guid.NewGuid().ToString("N"),
                Category = category,
            }
        };

        var db = new QueryDbContext();
        db.Users.AddRange(users);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));

        var query = db.Users
            .AddPropertyFilter(new PropertyFilter
            {
                Name = nameof(User.ExternalId),
                FilterType = FilterType.IS_NULL,
            })
            .AddPropertyFilter(new PropertyFilter
            {
                Name = nameof(User.CategoryId),
                FilterType = FilterType.EQ,
                Arguments = new object[] { category.Id }
            });
        
        var queryResults = query.ToList();
        
        Assert.That(
            queryResults.Count,
            Is.EqualTo(users.Count(u => u.ExternalId == null)));
        
        
        db.Users.RemoveRange(users);
        db.Categories.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));
        db.Dispose();
    }

    [Test]
    public void TestQueryByNullableColumnForIsNotNull()
    {
        var key = Guid.NewGuid().ToString("N");
        var category = new Category
        {
            Name = $"Group-{key}"
        };
        var users = new User[]
        {
            new()
            {
                Username = $"user1-{key}",
                Password = Guid.NewGuid().ToString("N"),
                ExternalId = 123456,
                Age = 30,
                Category = category,
            },
            
            new()
            {
                Username = $"user2-{key}",
                Password = Guid.NewGuid().ToString("N"),
                Age = 32,
                Category = category,
            },
            
            new()
            {
                Username = $"user3-{key}",
                Password = Guid.NewGuid().ToString("N"),
                Category = category,
            }
        };

        var db = new QueryDbContext();
        db.Users.AddRange(users);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));

        var query = db.Users
            .AddPropertyFilter(new PropertyFilter
            {
                Name = nameof(User.ExternalId),
                FilterType = FilterType.IS_NOT_NULL,
            })
            .AddPropertyFilter(new PropertyFilter
            {
                Name = nameof(User.CategoryId),
                FilterType = FilterType.EQ,
                Arguments = new object[] { category.Id }
            });
        
        var queryResults = query.ToList();
        
        Assert.That(
            queryResults.Count,
            Is.EqualTo(users.Count(u => u.ExternalId != null)));
        
        
        db.Users.RemoveRange(users);
        db.Categories.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));
        db.Dispose();
    }
    
    [Test]
    public void TestSimpleInsertQueryEqualToNullDelete()
    {
        var key = Guid.NewGuid().ToString("N");
        var category = new Category
        {
            Name = $"Group-{key}"
        };
        var users = new User[]
        {
            new()
            {
                Username = $"user1-{key}",
                Password = Guid.NewGuid().ToString("N"),
                ExternalId = 123456,
                Age = 30,
                Category = category,
            },
            
            new()
            {
                Username = $"user2-{key}",
                Password = Guid.NewGuid().ToString("N"),
                Age = 32,
                Category = category,
            },
            
            new()
            {
                Username = $"user3-{key}",
                Password = Guid.NewGuid().ToString("N"),
                Category = category,
            }
        };

        var db = new QueryDbContext();
        db.Users.AddRange(users);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));

        var query = db.Users
            .AddPropertyFilter(new PropertyFilter
            {
                Name = nameof(User.ExternalId),
                FilterType = FilterType.EQ,
                Arguments = new object[] { null! }
            })
            .AddPropertyFilter(new PropertyFilter
            {
                Name = nameof(User.CategoryId),
                FilterType = FilterType.EQ,
                Arguments = new object[] { category.Id }
            });
        
        var queryResults = query.ToList();
        
        Assert.That(
            queryResults.Count,
            Is.EqualTo(users.Count(u => u.ExternalId == null)));
        
        
        db.Users.RemoveRange(users);
        db.Categories.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));
        db.Dispose();
    }
    
    [Test]
    public void TestSimpleInsertQueryNotEqualToNullDelete()
    {
        var key = Guid.NewGuid().ToString("N");
        var category = new Category
        {
            Name = $"Group-{key}"
        };
        var users = new User[]
        {
            new()
            {
                Username = $"user1-{key}",
                Password = Guid.NewGuid().ToString("N"),
                ExternalId = 123456,
                Age = 30,
                Category = category,
            },
            
            new()
            {
                Username = $"user2-{key}",
                Password = Guid.NewGuid().ToString("N"),
                Age = 32,
                Category = category,
            },
            
            new()
            {
                Username = $"user3-{key}",
                Password = Guid.NewGuid().ToString("N"),
                Category = category,
            }
        };

        var db = new QueryDbContext();
        db.Users.AddRange(users);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));

        var query = db.Users
            .AddPropertyFilter(new PropertyFilter
            {
                Name = nameof(User.ExternalId),
                FilterType = FilterType.NOT_EQ,
                Arguments = new object[] { null! }
            })
            .AddPropertyFilter(new PropertyFilter
            {
                Name = nameof(User.CategoryId),
                FilterType = FilterType.EQ,
                Arguments = new object[] { category.Id }
            });
        
        var queryResults = query.ToList();
        
        Assert.That(
            queryResults.Count,
            Is.EqualTo(users.Count(u => u.ExternalId != null)));

        db.Users.RemoveRange(users);
        db.Categories.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));
        db.Dispose();
    }
    
    
    [Test]
    public void TestSimpleInsertQueryBlockDelete()
    {
        var key = Guid.NewGuid().ToString("N");
        var category = new Category
        {
            Name = $"Group-{key}"
        };
        var users = new User[]
        {
            new()
            {
                Username = $"user1-{key}",
                Password = Guid.NewGuid().ToString("N"),
                ExternalId = 123456,
                Age = 30,
                Category = category,
            },
            
            new()
            {
                Username = $"user2-{key}",
                Password = Guid.NewGuid().ToString("N"),
                Age = 32,
                Category = category,
            },
            
            new()
            {
                Username = $"user3-{key}",
                Password = Guid.NewGuid().ToString("N"),
                Category = category,
            }
        };

        var db = new QueryDbContext();
        db.Users.AddRange(users);
        var savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));

        var query = db.Users
            .AddPropertyFilterBlock(new PropertyFilterBlock
            {
                First = new PropertyFilter
                {
                    Name = nameof(User.ExternalId),
                    FilterType = FilterType.EQ,
                    Arguments = new object[] { 123456 }
                },
                Condition = BinaryOperator.Or,
                Others = new []
                {
                    new PropertyFilterBlock
                    {
                        First = new PropertyFilter
                        {
                            Name = nameof(User.ExternalId),
                            FilterType = FilterType.IS_NULL,
                            Arguments = new object[] { }
                        }
                    }
                }
            });
        
        var queryResults = query.ToList();
        
        Assert.That(
            queryResults.Count,
            Is.EqualTo(users.Count(u => u.ExternalId != null || u.ExternalId == null)));

        db.Users.RemoveRange(users);
        db.Categories.Remove(category);
        savedCount = db.SaveChanges();
        Assert.That(savedCount, Is.EqualTo(users.Length + 1));
        db.Dispose();
    }
}
