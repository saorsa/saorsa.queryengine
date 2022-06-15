using Microsoft.AspNetCore.Mvc;

namespace Saorsa.QueryEngine.API.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public ActionResult<ResultRef> Get()
    {
        return Ok(new ResultRef
        {
            Status = ResultStatus.Ok,
            Message = "Health endpoint result success!",
        });
    }
}
