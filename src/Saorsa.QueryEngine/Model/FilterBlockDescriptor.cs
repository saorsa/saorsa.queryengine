namespace Saorsa.QueryEngine.Model;


/// <summary>
/// A DTO object that represents a filter block - a set of individual property filters, all of which are
/// evaluated with a shared logical operator (AND -or- OR).
/// </summary>
public class FilterBlockDescriptor
{
    /// <summary>
    /// Gets or sets the first property descriptor. Mandatory.
    /// </summary>
    public FilterPropertyDescriptor First { get; set; } = default!;

    /// <summary>
    /// Gets or sets the logical operator to be applied for all arguments in the filter block. Used only
    /// if there are Other's filters defined.
    /// </summary>
    public LogicalOperator? Condition { get; set; }

    /// <summary>
    /// Gets or sets the reminder of property filters to be used along with the condition.
    /// </summary>
    public IEnumerable<FilterBlockDescriptor> Others { get; set; } = Array.Empty<FilterBlockDescriptor>();

    /// <summary>
    /// Gets an indication, if there is more than one conditional filter in the block.
    /// </summary>
    public bool IsComposite => Condition.HasValue && Others.Any();
}
