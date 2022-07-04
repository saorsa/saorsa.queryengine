using Saorsa.Model;

namespace Saorsa.QueryEngine.Model;

public class QueryEnginePageResult<TRequest, TId, TPageItem> : BusinessPageResult<
    TRequest, TId, TPageItem>
    where TRequest : BusinessPageRequest<TId>
{
}