namespace Saorsa.QueryEngine.Model;

public class PropertyDefinition
{
    public string Name { get; set; } = $"property_{Guid.NewGuid():N}";

    public string? Type { get; set; }
}
