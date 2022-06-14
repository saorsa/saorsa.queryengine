namespace Saorsa.QueryEngine.Model;

public class PropertyFilterBlock
{
    public PropertyFilter First { get; set; } = default!;

    public BinaryOperator? Condition { get; set; }
    
    public IEnumerable<PropertyFilterBlock>? Others { get; set; }
}
