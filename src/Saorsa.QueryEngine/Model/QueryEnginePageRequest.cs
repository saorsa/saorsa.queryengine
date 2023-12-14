using Saorsa.Model;

namespace Saorsa.QueryEngine.Model;


/// <summary>
/// Represents a DTO object for a composite business request with a dynamic property filter block expression
/// </summary>
/// <typeparam name="TId">
/// The underlying type of the unique identifier of the entities that are filtered by the operation.
/// </typeparam>
public class QueryEnginePageRequest<TId> : BusinessPageRequest<TId>
{
    /// <summary>
    /// Gets or sets the filter block expression to be used during request filtering.
    /// </summary>
    public FilterBlockDescriptor? FilterExpression { get; set; }
}
