namespace Saorsa.QueryEngine.Model;


/// <summary>
/// Object that carries meta description about an property type that can be used in dynamic queries used by
/// the Query Engine runtime.
/// </summary>
public class QueryableTypeDescriptor
{
    public string Name { get; set; } = $"name_{Guid.NewGuid():N}";
    
    public string TypeName { get; set; } = $"type_{Guid.NewGuid():N}";

    public bool Nullable { get; set; }

    public string? Type { get; set; }
    
    public string[]? EnumValues { get; set; }
    
    public QueryableTypeDescriptor[]? Properties { get; set; } 

    public ExpressionDescriptor[] AllowedFilters { get; set; } = Array.Empty<ExpressionDescriptor>();
    
    public QueryableTypeDescriptor? ArrayElement { get; set; }

    public override string ToString()
    {
        return $"[Type = {TypeName}, {Type}{(Nullable ? "?" : string.Empty)}]";
    }
}
