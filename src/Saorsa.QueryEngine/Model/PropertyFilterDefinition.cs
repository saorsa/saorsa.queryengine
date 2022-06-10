namespace Saorsa.QueryEngine.Model;

public class PropertyFilterDefinition
{
    public string FilterType { get; }
    
    public object? Arg1 { get; }
    
    public object? Arg2 { get; }

    public PropertyFilterDefinition(string type, object? arg1 = default, object? arg2 = default)
    {
        FilterType = type;
        Arg1 = arg1;
        Arg2 = arg2;
    }
}
