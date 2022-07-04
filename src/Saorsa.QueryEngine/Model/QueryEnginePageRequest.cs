using Saorsa.Model;

namespace Saorsa.QueryEngine.Model;

public class QueryEnginePageRequest<TId> : BusinessPageRequest<TId>
{
    public PropertyFilterBlock? FilterExpression { get; set; }
}

