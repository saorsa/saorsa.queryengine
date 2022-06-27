using Microsoft.AspNetCore.Mvc;
using Saorsa.QueryEngine.Model;

namespace Saorsa.QueryEngine.API.Controllers;

[ApiController]
[Route("[controller]")]
public class MetaController : ControllerBase
{
    [HttpGet("scan")]
    public ActionResult<ResultRef<TypeDefinition[]>> GetScannedTypes()
    {
        var types = QueryEngine.ScanQueryEngineTypes();
        var scanned = types
            .Select(t => QueryEngine.BuildTypeDefinition(t))
            .Where(t => t != null)
            .Select(t => t!)
            .ToArray();
        
        return Ok(new ResultRef<TypeDefinition[]>
        {
            Status = ResultStatus.Ok,
            Result = scanned,
        });
    }
    
    [HttpGet("compile/{typeName}")]
    public ActionResult<ResultRef<bool>> Compile(string typeName)
    {
        var type = Type.GetType(typeName);
        if (type == null)
        {
            return BadRequest(new ResultRef<bool>
            {
                Status = ResultStatus.Error,
                Message = $"Type '{typeName}' cannot be resolved.",
                Result = false,
            });
        }
        
        var typeDef = QueryEngine.EnsureCompiled(type);

        if (typeDef == null)
        {
            return BadRequest(new ResultRef<bool>
            {
                Status = ResultStatus.Error,
                Message = $"Type '{typeName}' is resolved but cannot be compiled.",
                Result = false,
            });
        }
        
        return Ok(new ResultRef<bool>
        {
            Status = ResultStatus.Ok,
            Message = $"Type '{typeName}' is compiled and ready for queries.",
            Result = true
        });
    }
    
    [HttpGet("cached")]
    public ActionResult<ResultRef<TypeDefinition[]>> GetCachedTypeDefs()
    {
        var types = QueryEngine.ScanQueryEngineTypes();
        var cached = types
            .Where(t => QueryEngine.IsCompiled(t))
            .Select(t => QueryEngine.GetCompiled(t)!)
            .ToArray();
        
        return Ok(new ResultRef<TypeDefinition[]>
        {
            Status = ResultStatus.Ok,
            Result = cached,
        });
    }
    
    [HttpGet("cached/{typeName}")]
    public ActionResult<ResultRef<TypeDefinition>> GetCachedTypeDef(string typeName)
    {
        var types = QueryEngine.ScanQueryEngineTypes();
        var cached = types
            .Where(t => QueryEngine.IsCompiled(t))
            .Select(t => QueryEngine.GetCompiled(t)!)
            .FirstOrDefault(t => t.Name.Equals(typeName));

        if (cached != null)
        {
            return Ok(new ResultRef<TypeDefinition>
            {
                Status = ResultStatus.Ok,
                Result = cached,
            });
        }
        
        return NotFound(new ResultRef<TypeDefinition>
        {
            Status = ResultStatus.Fatal,
            Message = $"Type '{typeName}' is not recognized by Query Engine."
        });
    }
}
