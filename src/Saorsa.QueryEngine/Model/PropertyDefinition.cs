namespace Saorsa.QueryEngine.Model;

public class PropertyDefinition
{
    public string Name { get; set; } = $"property_{Guid.NewGuid():N}";

    public bool Nullable { get; set; }

    public string? Type { get; set; }

    public override string ToString()
    {
        return $"[{Name} {Type}{(Nullable ? "?" : string.Empty)}]";
    }
}
