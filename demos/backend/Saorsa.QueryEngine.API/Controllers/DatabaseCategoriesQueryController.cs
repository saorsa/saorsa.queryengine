using Microsoft.AspNetCore.Mvc;
using Saorsa.QueryEngine.Model;
using Saorsa.QueryEngine.Tests.EFCore.Entities;
using Saorsa.QueryEngine.Tests.NpgSql.Data;

namespace Saorsa.QueryEngine.API.Controllers;

[ApiController]
[Route("database/categories")]
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
        var result = Db.Categories.DynamicSearch<QueryEnginePageRequest<int>, int, Category>(pageRequest);

        return Ok(result);
    }
}
