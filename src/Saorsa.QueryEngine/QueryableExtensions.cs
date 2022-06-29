using Saorsa.QueryEngine.Model;

namespace Saorsa.QueryEngine;

public static class QueryableExtensions
{
    public static IQueryable<TEntity> Where<TEntity>(
        this IQueryable<TEntity> source,
        PropertyFilter propertyFilter)
    {
        return QueryEngine.AddPropertyFilter(source, propertyFilter);
    }
    
    public static IQueryable<TEntity> Where<TEntity>(
        this IQueryable<TEntity> source,
        PropertyFilterBlock block)
    {
        return QueryEngine.AddPropertyFilterBlock(source, block);
    }
}
