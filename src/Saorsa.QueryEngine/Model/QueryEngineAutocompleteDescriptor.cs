namespace Saorsa.QueryEngine.Model;


/// <summary>
/// A simple DTO model that generalizes the simplest view for a target entity.
/// </summary>
public class QueryEngineAutocompleteDescriptor
{
    /// <summary>
    /// Gets or sets the unique ID of the object instance.
    /// </summary>
    public object? Id { get; set; }

    /// <summary>
    /// Gets or sets the unique key of the object instance.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Gets or sets the UI label that is associated with the key of the object instance.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets a description text associated with the object instance.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets an URL with external data for the object.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the image URL for the object.
    /// </summary>
    public string? ImageUrl { get; set; }
}
