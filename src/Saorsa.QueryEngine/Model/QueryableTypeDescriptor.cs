namespace Saorsa.QueryEngine.Model;


/// <summary>
/// Object that carries meta description about a type that can be used in dynamic queries used by
/// the Query Engine runtime.
/// </summary>
public class QueryableTypeDescriptor
{
    /// <summary>
    /// Gets or sets the name of the queryable item. If the item is an object class / struct/ enum, its label is used.
    /// If the item is an object class property, the property name is used.
    /// </summary>
    public string Name { get; set; } = $"name_{Guid.NewGuid():N}";

    /// <summary>
    /// Gets or sets the underlying CLR type name of the queryable item as a string.
    /// </summary>
    public string TypeName { get; set; } = $"type_{Guid.NewGuid():N}";

    /// <summary>
    /// Gets or sets an indication whether this queryable item allows null values or not.
    /// </summary>
    public bool Nullable { get; set; }

    /// <summary>
    /// Gets or sets the query engine type associated with the queryable.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the array of allowed enumeration values, if the queryable item represents an enum.
    /// </summary>
    public string[]? EnumValues { get; set; }

    /// <summary>
    /// Gets or sets the array of property descriptors, if the queryable item represents an object.
    /// </summary>
    public QueryableTypeDescriptor[]? Properties { get; set; } 

    /// <summary>
    /// Gets or sets the the property descriptor for elements of the array, if the queryable item represents an array.
    /// </summary>
    public QueryableTypeDescriptor? ArrayElement { get; set; }

    /// <summary>
    /// Gets or sets the collection of allowed filter expressions for this queryable item.
    /// </summary>
    public FilterMetaData[] AllowedFilters { get; set; } = Array.Empty<FilterMetaData>();

    /// <summary>
    /// Gets the string representation of the descriptor.
    /// </summary>
    public override string ToString()
    {
        return $"[Type = {TypeName}, {Type}{(Nullable ? "?" : string.Empty)}]";
    }
}
