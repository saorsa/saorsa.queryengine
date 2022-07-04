using Saorsa.Model;
using Saorsa.QueryEngine.Model;
using Saorsa.QueryEngine.Tests.EFCore.Entities;

namespace Saorsa.QueryEngine.API.Controllers;

public class ResultCategories : BusinessPageResult<QueryEnginePageRequest<int>, int, Category>
{
}
