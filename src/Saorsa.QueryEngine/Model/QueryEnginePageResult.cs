using Saorsa.Model;

namespace Saorsa.QueryEngine.Model;


/// <summary>
/// Represents a DTO object for a composite business result from a query engine filter request. 
/// </summary>
/// <typeparam name="TRequest">
/// The type of the input filter for the business operation.
/// </typeparam>
/// <typeparam name="TId">
/// The underlying type of the unique identifier of the entities being filtered.
/// </typeparam>
/// <typeparam name="TPageItem">
/// The type of the items in the result page set.
/// </typeparam>
public class QueryEnginePageResult<TRequest, TId, TPageItem> : BusinessPageResult<
    TRequest, TId, TPageItem>
    where TRequest : BusinessPageRequest<TId>
{
}
