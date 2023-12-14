using Microsoft.AspNetCore.Mvc;
using Saorsa.QueryEngine.Model;
using Saorsa.QueryEngine.Tests.EFCore.Entities;
using Saorsa.QueryEngine.Tests.EFCore.NpgSql;

namespace Saorsa.QueryEngine.API.Controllers;

[ApiController]
[Route("database-hardcoded/users")]
public class DatabaseUsersQueryController : ControllerBase
{
    public QueryNpgsqlDbContext Db { get; }

    public DatabaseUsersQueryController(QueryNpgsqlDbContext db)
    {
        Db = db;
    }
    
    [HttpPost("query")]
    public ActionResult<ResultUsers> Query([FromBody] QueryEnginePageRequest<Guid> pageRequest)
    {
        var result = Db.Users.DynamicSearch<QueryEnginePageRequest<Guid>, Guid, User>(pageRequest);

        return Ok(result);
    }
}