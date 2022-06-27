using Saorsa.Model;

namespace Saorsa.QueryEngine.Model;

public class QueryEnginePageRequest<T> : BusinessPageRequest<T>
{
    public PropertyFilterBlock? FilterExpression { get; set; }
}

