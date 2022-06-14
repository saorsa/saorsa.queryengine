using Saorsa.QueryEngine.Model;

namespace Saorsa.QueryEngine;

public static class QueryableExtensions
{
    public static IQueryable<TEntity> AddPropertyFilter<TEntity>(
        this IQueryable<TEntity> source,
        PropertyFilter propertyFilter)
    {
        return QueryEngine.AddPropertyFilter(source, propertyFilter);
    }
}
