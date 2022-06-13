using Saorsa.Model;

namespace Saorsa.QueryEngine.Model;

public class QueryEnginePageRequest<T> : BusinessPageRequest<T>
{
    public PropertyFilter[] PropertyFilters { get; set; } = Array.Empty<PropertyFilter>();
}
