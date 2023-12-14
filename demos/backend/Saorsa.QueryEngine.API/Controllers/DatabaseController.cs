using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saorsa.QueryEngine.Tests.EFCore.Entities;
using Saorsa.QueryEngine.Tests.EFCore.NpgSql;

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
                        new { rootCategory.Name, rootCategory.Id,
                            ParentCategoryId = rootCategory.ParentDepartmentId, 
                            FillType = CategoryStates[rootCategory.Id] },
                        new { groupsRootCategory.Name, groupsRootCategory.Id,
                            ParentCategoryId = groupsRootCategory.ParentDepartmentId,
                            FillType = CategoryStates[groupsRootCategory.Id] },
                        new { devGroupCategory.Name, devGroupCategory.Id,
                            ParentCategoryId = devGroupCategory.ParentDepartmentId,
                            FillType = CategoryStates[devGroupCategory.Id] },
                        new { qaGroupCategory.Name, qaGroupCategory.Id,
                            ParentCategoryId = qaGroupCategory.ParentDepartmentId,
                            FillType = CategoryStates[qaGroupCategory.Id] },
                        new { usersRootCategory.Name, usersRootCategory.Id,
                            ParentCategoryId = usersRootCategory.ParentDepartmentId,
                            FillType = CategoryStates[usersRootCategory.Id] },
                        new { adminUsersCategory.Name, adminUsersCategory.Id,
                            ParentCategoryId = adminUsersCategory.ParentDepartmentId,
                            FillType = CategoryStates[adminUsersCategory.Id] },
                        new { guestUsersCategory.Name, guestUsersCategory.Id,
                            ParentCategoryId = guestUsersCategory.ParentDepartmentId,
                            FillType = CategoryStates[guestUsersCategory.Id] },
                    },
                    Groups = new []
                    {
                        new { organisationGroup.Id,
                            CategoryId = organisationGroup.OwnerDepartmentId,
                            FillType = GroupStates[organisationGroup.Id] },
                        new { adminUsersGroup.Id,
                            CategoryId = adminUsersGroup.OwnerDepartmentId,
                            FillType = GroupStates[adminUsersGroup.Id] },
                        new { guestUsersGroup.Id,
                            CategoryId = guestUsersGroup.OwnerDepartmentId,
                            FillType = GroupStates[guestUsersGroup.Id] }
                    },
                    Users = new []
                    {
                        new { rootUser.Id, rootUser.Username, rootUser.Gender, rootUser.ExternalId,
                            CategoryId = rootUser.DepartmentId, rootUser.LatestLogonType,
                            Groups = rootUser.Tags.Select(g => g.Id),
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

    protected Department EnsureCategory(string categoryName, int? parentCategoryId = null)
    {
        if (parentCategoryId != null)
        {
            var parent = Db.Departments.FirstOrDefault(c => c.Id == parentCategoryId.Value);
            if (parent == null)
            {
                throw new ArgumentException($"Category {parentCategoryId} could not be resolved.");
            }
        }

        var existing = Db.Departments
            .FirstOrDefault(c => c.Name.Equals(categoryName) && (
                parentCategoryId.HasValue ? c.ParentDepartmentId == parentCategoryId : c.ParentDepartmentId == null
            ));

        if (existing == null)
        {
            existing = new Department
            {
                Name = categoryName,
                ParentDepartmentId = parentCategoryId,
            };

            Db.Departments.Add(existing);
            Db.SaveChanges();
            CategoryStates.Add(existing.Id, DataFillType.Insert);
        }
        else if (existing.ParentDepartmentId != parentCategoryId)
        {
            existing.ParentDepartmentId = parentCategoryId;
            Db.Departments.Update(existing);
            Db.SaveChanges();
            CategoryStates.Add(existing.Id, DataFillType.Update);
        }
        else
        {
            CategoryStates.Add(existing.Id, DataFillType.Skip);
        }

        return existing;
    }

    protected Tag EnsureGroup(string identifier, int? categoryId = null)
    {
        if (categoryId != null)
        {
            var parent = Db.Departments.FirstOrDefault(c => c.Id == categoryId.Value);
            if (parent == null)
            {
                throw new ArgumentException($"Category {categoryId} could not be resolved.");
            }
        }

        var existingGroup = Db.Tags.FirstOrDefault(g => g.Id == identifier);

        if (existingGroup == null)
        {
            existingGroup = new Tag()
            {
                Id = identifier,
                OwnerDepartmentId = categoryId,
            };
            Db.Tags.Add(existingGroup);
            Db.SaveChanges();
            GroupStates.Add(existingGroup.Id, DataFillType.Insert);
        }
        else if (existingGroup.OwnerDepartmentId != categoryId)
        {   
            existingGroup.OwnerDepartmentId = categoryId;
            Db.Tags.Update(existingGroup);
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
            var parent = Db.Departments.FirstOrDefault(c => c.Id == categoryId.Value);
            if (parent == null)
            {
                throw new ArgumentException($"Category {categoryId} could not be resolved.");
            }
        }

        var parentGroups = new List<Tag>();
        var parentGroupIds = new List<string>();
        
        if (groupIds != null)
        {
            parentGroups = Db.Tags.Where(g => groupIds.Contains(g.Id)).ToList();
            parentGroupIds = parentGroups.Select(g => g.Id).ToList();

            if (parentGroupIds.Count != groupIds.Length)
            {
                throw new ArgumentException(
                    $"One or more group input identifiers ({string.Join(',', groupIds)}) are invalid. The " +
                    $"matched ones are {string.Join(',', parentGroupIds)}.");
            }
        }
        

        var existingUser = Db.Users
            .Include(u => u.Tags)
            .FirstOrDefault(u => u.Username == username);

        if (existingUser == null)
        {
            existingUser = new User()
            {
                Username = username,
                DepartmentId = categoryId,
            };
            parentGroups.ForEach(g =>
            {
                existingUser.Tags.Add(g);
            });
            Db.Users.Add(existingUser);
            Db.SaveChanges();
            UserStates.Add(existingUser.Username, DataFillType.Insert);
        }
        else if (existingUser.DepartmentId != categoryId
                 || !existingUser.Tags.All(g => parentGroupIds.Contains(g.Id))
                 || !parentGroupIds.All(g => existingUser.Tags.Select(eug => eug.Id).Contains(g)))
        {   
            existingUser.DepartmentId = categoryId;
            
            existingUser.Tags.Clear();
            parentGroups.ForEach(g =>
            {
                if (existingUser.Tags.All(existingGroup => g.Id != existingGroup.Id))
                {
                    existingUser.Tags.Add(g);
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
