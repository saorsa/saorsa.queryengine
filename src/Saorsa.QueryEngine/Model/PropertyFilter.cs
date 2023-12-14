namespace Saorsa.QueryEngine.Model;

public class PropertyFilter
{
    public string Name { get; set; } = $"Property_{Guid.NewGuid():N}";

    public FilterOperatorType FilterType { get; set; }

    public object[] Arguments { get; set; } = Array.Empty<object>();
}
