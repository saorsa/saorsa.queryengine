using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saorsa.QueryEngine.Tests.NpgSql.Data;

namespace Saorsa.QueryEngine.API.Controllers;

[ApiController]
[Route("[controller]")]
public class DatabaseController : ControllerBase
{
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
}
