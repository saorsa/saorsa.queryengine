using Microsoft.AspNetCore.Mvc;
using Saorsa.QueryEngine.Model;
using Saorsa.QueryEngine.Tests.EFCore.Entities;
using Saorsa.QueryEngine.Tests.EFCore.NpgSql;

namespace Saorsa.QueryEngine.API.Controllers;

[GenericControllerNameConvention]
[Route("database/[controller]")]
public class GenericController<T> : ControllerBase where T : class
{
    public QueryNpgsqlDbContext Db { get; }

    public GenericController(QueryNpgsqlDbContext db)
    {
        Db = db;
    }

    [Route("")]
    [HttpGet]
    public IActionResult Index()
    {
        return Content($"Hello from a generic {typeof(T).Name} controller.");
    }
    
    [HttpGet("query")]
    public ActionResult<ResultUsers> Query()
    {
        var result = Db.Set<T>().ToList();

        return Ok(result);
    }

    [HttpPost("query")]
    public ActionResult<ResultUsers> Query([FromBody]QueryEnginePageRequest<int> pageRequest)
    {
        var result = Db.Set<T>().DynamicSearch<QueryEnginePageRequest<int>, int, T>(pageRequest);

        return Ok(result);
    }
}
