using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Saorsa.QueryEngine.Model;
using Saorsa.QueryEngine.Tests.NpgSql.Data;

namespace Saorsa.QueryEngine.API.Controllers;

[ApiController]
[Route("database/users")]
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
        var query = Db.Users.AsQueryable();

        if (pageRequest.FilterExpression != null)
        {
            query = query.AddPropertyFilterBlock(pageRequest.FilterExpression);
        }

        var totalCount = query.LongCount();
        var pageSize = Convert.ToInt32(pageRequest.PageSize ?? 10);
        var pageIndex = Convert.ToInt32(pageRequest.PageIndex ?? 0);

        query = query
            .Skip(pageSize * pageIndex)
            .Take(pageSize);

        var pageResults = query.ToList();
        
        var result = new ResultUsers
        {
            Context = pageRequest,
            TotalCount = Convert.ToUInt64(totalCount),
            Result = pageResults,
        };

        return Ok(result);
    }
}