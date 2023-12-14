using Microsoft.AspNetCore.Mvc;
using Saorsa.QueryEngine.Model;
using Saorsa.QueryEngine.Tests.EFCore.Entities;
using Saorsa.QueryEngine.Tests.EFCore.NpgSql;

namespace Saorsa.QueryEngine.API.Controllers;

[ApiController]
[Route("database-hardcoded/categories")]
public class DatabaseCategoriesQueryController: ControllerBase
{
    public QueryNpgsqlDbContext Db { get; }

    public DatabaseCategoriesQueryController(QueryNpgsqlDbContext db)
    {
        Db = db;
    }
    
    [HttpPost("query")]
    public ActionResult<ResultUsers> Query([FromBody]QueryEnginePageRequest<int> pageRequest)
    {
        var result = Db.Departments.DynamicSearch<QueryEnginePageRequest<int>, int, Department>(pageRequest);

        return Ok(result);
    }
}
