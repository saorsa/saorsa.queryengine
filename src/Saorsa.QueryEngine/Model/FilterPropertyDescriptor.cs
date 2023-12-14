namespace Saorsa.QueryEngine.Model;


/// <summary>
/// A DTO that represents a generalized filter for a single object property.
/// </summary>
public class FilterPropertyDescriptor
{
    /// <summary>
    /// Gets or sets the name of the property to be added during the filter request.
    /// </summary>
    public string Name { get; set; } = $"Property_{Guid.NewGuid():N}";

    /// <summary>
    /// Gets or sets the type of the filter operation used against the property.
    /// </summary>
    public FilterOperatorType FilterType { get; set; }

    /// <summary>
    /// Gets or sets the arguments of the filter operation.
    /// </summary>
    public object[] Arguments { get; set; } = Array.Empty<object>();

    /// <summary>
    /// Gets an indication, if this filter property is a nested property.
    /// </summary>
    public bool IsNestedProperty => Name.Contains(".");

    /// <summary>
    /// Gets the sequence / property path for the property. This is mostly used with nested / aggregated
    /// properties.
    /// </summary>
    public string[] AsSequence()
    {
        return IsNestedProperty
            ? Name.Split(".")
            : new[] { Name };
    }
}
