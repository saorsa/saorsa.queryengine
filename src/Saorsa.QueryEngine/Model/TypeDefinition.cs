namespace Saorsa.QueryEngine.Model;

public class TypeDefinition
{
    public string Name { get; set; } = $"name_{Guid.NewGuid():N}";
    
    public string TypeName { get; set; } = $"type_{Guid.NewGuid():N}";

    public bool Nullable { get; set; }

    public string? Type { get; set; }
    
    public string[]? EnumValues { get; set; }
    
    public TypeDefinition[]? Properties { get; set; } 

    public FilterDefinition[] AllowedFilters { get; set; } = Array.Empty<FilterDefinition>();
    
    public TypeDefinition? ArrayElement { get; set; }

    public override string ToString()
    {
        return $"[Type = {TypeName}, {Type}{(Nullable ? "?" : string.Empty)}]";
    }
}
