namespace Saorsa.QueryEngine.Model;

public class PropertyFilterBlock
{
    public PropertyFilter First { get; set; } = default!;

    public LogicalOperator? Condition { get; set; }

    public IEnumerable<PropertyFilterBlock> Others { get; set; } = Array.Empty<PropertyFilterBlock>();

    public bool IsComposite => Condition.HasValue && Others.Any();
}
