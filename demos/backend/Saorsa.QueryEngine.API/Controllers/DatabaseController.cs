using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saorsa.QueryEngine.Tests.EFCore.Entities;
using Saorsa.QueryEngine.Tests.NpgSql.Data;

namespace Saorsa.QueryEngine.API.Controllers;

[ApiController]
[Route("[controller]")]
public class DatabaseController : ControllerBase
{
    enum DataFillType
    {
        Insert,
        Update,
        Skip,
    }
    
    private readonly Dictionary<int, DataFillType> CategoryStates = new();
    private readonly Dictionary<string, DataFillType> GroupStates = new();
    private readonly Dictionary<string, DataFillType> UserStates = new();

    public QueryNpgsqlDbContext Db { get; }

    public DatabaseController(QueryNpgsqlDbContext db)
    {
        Db = db;
    }

    [HttpGet("probe")]
    public ActionResult<ResultRef> CanConnect()
    {
        try
        {
            var canConnect = Db.Database.CanConnect();
            return Ok(new ResultRef
            {
                Status = canConnect ? ResultStatus.Ok : ResultStatus.Error,
                Message = canConnect ? "Database connection established" : "Failed to connect to the sample database",
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResultRef
            {
                Status = ResultStatus.Fatal,
                Message = e.Message,
                RefType = $"Exception: {e.GetType().FullName}",
            });
        }
    }

    [HttpGet("migrate")]
    public ActionResult<ResultRef> Migrate()
    {
        try
        {
            Db.Database.Migrate();
            return Ok(new
            {
                Status = "OK",
                Message = "Database changes migrated successfully"
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                Status = "FATAL",
                Error = e.Message,
                RefType = e.GetType().FullName,
            });
        }
    }

    [HttpGet("data/fill")]
    public ActionResult<ResultRef> FillData()
    {
        try
        {
            // Categories
            
            var rootCategory = EnsureCategory("ROOT");
            var groupsRootCategory = EnsureCategory("GROUPS", rootCategory.Id);
            var devGroupCategory = EnsureCategory("DEV", groupsRootCategory.Id);
            var qaGroupCategory = EnsureCategory("QA", groupsRootCategory.Id);
            var usersRootCategory = EnsureCategory("USERS", rootCategory.Id);
            var adminUsersCategory = EnsureCategory("ADMINS", usersRootCategory.Id);
            var guestUsersCategory = EnsureCategory("GUESTS", usersRootCategory.Id);
            
            // Groups

            var organisationGroup = EnsureGroup("Organisation", groupsRootCategory.Id);
            var adminUsersGroup = EnsureGroup("AdministrativeUsers", adminUsersCategory.Id);
            var guestUsersGroup = EnsureGroup("GuestUsers", guestUsersCategory.Id);
            
            // Users

            var rootUser = EnsureUser("root", adminUsersCategory.Id, new[]
            {
                organisationGroup.Id,
                adminUsersGroup.Id
            });
            
            return Ok(new ResultRef
            {
                Status = ResultStatus.Ok,
                Code = 200,
                Message = "Database data fill-in successful",
                Context = new
                {
                    Categories = new[]
                    {
                        new { rootCategory.Name, rootCategory.Id, rootCategory.ParentCategoryId, 
                            FillType = CategoryStates[rootCategory.Id] },
                        new { groupsRootCategory.Name, groupsRootCategory.Id, groupsRootCategory.ParentCategoryId,
                            FillType = CategoryStates[groupsRootCategory.Id] },
                        new { devGroupCategory.Name, devGroupCategory.Id, devGroupCategory.ParentCategoryId,
                            FillType = CategoryStates[devGroupCategory.Id] },
                        new { qaGroupCategory.Name, qaGroupCategory.Id, qaGroupCategory.ParentCategoryId,
                            FillType = CategoryStates[qaGroupCategory.Id] },
                        new { usersRootCategory.Name, usersRootCategory.Id, usersRootCategory.ParentCategoryId,
                            FillType = CategoryStates[usersRootCategory.Id] },
                        new { adminUsersCategory.Name, adminUsersCategory.Id, adminUsersCategory.ParentCategoryId,
                            FillType = CategoryStates[adminUsersCategory.Id] },
                        new { guestUsersCategory.Name, guestUsersCategory.Id, guestUsersCategory.ParentCategoryId,
                            FillType = CategoryStates[guestUsersCategory.Id] },
                    },
                    Groups = new []
                    {
                        new { organisationGroup.Id, organisationGroup.CategoryId,
                            FillType = GroupStates[organisationGroup.Id] },
                        new { adminUsersGroup.Id, adminUsersGroup.CategoryId,
                            FillType = GroupStates[adminUsersGroup.Id] },
                        new { guestUsersGroup.Id, guestUsersGroup.CategoryId,
                            FillType = GroupStates[guestUsersGroup.Id] }
                    },
                    Users = new []
                    {
                        new { rootUser.Id, rootUser.Username, rootUser.Gender, rootUser.ExternalId, 
                            rootUser.CategoryId, rootUser.LatestLogonType,
                            Groups = rootUser.Groups.Select(g => g.Id),
                            FillType = UserStates[rootUser.Username]
                        }
                    }
                },
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new ResultRef
            {
                Status = ResultStatus.Error,
                Message = e.Message,
                Code = 500,
                RefType = e.GetType().FullName,
            });
        }
    }

    protected Category EnsureCategory(string categoryName, int? parentCategoryId = null)
    {
        if (parentCategoryId != null)
        {
            var parent = Db.Categories.FirstOrDefault(c => c.Id == parentCategoryId.Value);
            if (parent == null)
            {
                throw new ArgumentException($"Category {parentCategoryId} could not be resolved.");
            }
        }

        var existing = Db.Categories
            .FirstOrDefault(c => c.Name.Equals(categoryName) && (
                parentCategoryId.HasValue ? c.ParentCategoryId == parentCategoryId : c.ParentCategoryId == null
            ));

        if (existing == null)
        {
            existing = new Category
            {
                Name = categoryName,
                ParentCategoryId = parentCategoryId,
            };

            Db.Categories.Add(existing);
            Db.SaveChanges();
            CategoryStates.Add(existing.Id, DataFillType.Insert);
        }
        else if (existing.ParentCategoryId != parentCategoryId)
        {
            existing.ParentCategoryId = parentCategoryId;
            Db.Categories.Update(existing);
            Db.SaveChanges();
            CategoryStates.Add(existing.Id, DataFillType.Update);
        }
        else
        {
            CategoryStates.Add(existing.Id, DataFillType.Skip);
        }

        return existing;
    }

    protected Group EnsureGroup(string identifier, int? categoryId = null)
    {
        if (categoryId != null)
        {
            var parent = Db.Categories.FirstOrDefault(c => c.Id == categoryId.Value);
            if (parent == null)
            {
                throw new ArgumentException($"Category {categoryId} could not be resolved.");
            }
        }

        var existingGroup = Db.Groups.FirstOrDefault(g => g.Id == identifier);

        if (existingGroup == null)
        {
            existingGroup = new Group()
            {
                Id = identifier,
                CategoryId = categoryId,
            };
            Db.Groups.Add(existingGroup);
            Db.SaveChanges();
            GroupStates.Add(existingGroup.Id, DataFillType.Insert);
        }
        else if (existingGroup.CategoryId != categoryId)
        {   
            existingGroup.CategoryId = categoryId;
            Db.Groups.Update(existingGroup);
            Db.SaveChanges();
            GroupStates.Add(existingGroup.Id, DataFillType.Update);
        }
        else
        {
            GroupStates.Add(existingGroup.Id, DataFillType.Skip);
        }

        return existingGroup;
    }

    protected User EnsureUser(string username, int? categoryId = null, string[]? groupIds = null)
    {
        if (categoryId != null)
        {
            var parent = Db.Categories.FirstOrDefault(c => c.Id == categoryId.Value);
            if (parent == null)
            {
                throw new ArgumentException($"Category {categoryId} could not be resolved.");
            }
        }

        var parentGroups = new List<Group>();
        var parentGroupIds = new List<string>();
        
        if (groupIds != null)
        {
            parentGroups = Db.Groups.Where(g => groupIds.Contains(g.Id)).ToList();
            parentGroupIds = parentGroups.Select(g => g.Id).ToList();

            if (parentGroupIds.Count != groupIds.Length)
            {
                throw new ArgumentException(
                    $"One or more group input identifiers ({string.Join(',', groupIds)}) are invalid. The " +
                    $"matched ones are {string.Join(',', parentGroupIds)}.");
            }
        }
        

        var existingUser = Db.Users
            .Include(u => u.Groups)
            .FirstOrDefault(u => u.Username == username);

        if (existingUser == null)
        {
            existingUser = new User()
            {
                Username = username,
                CategoryId = categoryId,
            };
            parentGroups.ForEach(g =>
            {
                existingUser.Groups.Add(g);
            });
            Db.Users.Add(existingUser);
            Db.SaveChanges();
            UserStates.Add(existingUser.Username, DataFillType.Insert);
        }
        else if (existingUser.CategoryId != categoryId
                 || !existingUser.Groups.All(g => parentGroupIds.Contains(g.Id))
                 || !parentGroupIds.All(g => existingUser.Groups.Select(eug => eug.Id).Contains(g)))
        {   
            existingUser.CategoryId = categoryId;
            
            existingUser.Groups.Clear();
            parentGroups.ForEach(g =>
            {
                if (existingUser.Groups.All(existingGroup => g.Id != existingGroup.Id))
                {
                    existingUser.Groups.Add(g);
                }
            });
            
            Db.Users.Update(existingUser);
            Db.SaveChanges();
            UserStates.Add(existingUser.Username, DataFillType.Update);
        }
        else
        {
            UserStates.Add(existingUser.Username, DataFillType.Skip);
        }

        return existingUser;
    }
}
