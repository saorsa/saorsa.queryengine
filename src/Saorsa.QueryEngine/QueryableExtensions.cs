using Saorsa.Model;
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
    
    public static BusinessPageResult<TRequest, TId, TEntity>
        DynamicSearch<TRequest, TId, TEntity>(
        this IQueryable<TEntity> source,
        TRequest pageRequest)
        where TRequest: QueryEnginePageRequest<TId>
    {
        if (pageRequest.FilterExpression != null)
        {
            source = source.Where(pageRequest.FilterExpression);
        }

        var totalCount = source.LongCount();
        var pageSize = Convert.ToInt32(pageRequest.PageSize ?? 10);
        var pageIndex = Convert.ToInt32(pageRequest.PageIndex ?? 0);

        source = source
            .Skip(pageSize * pageIndex)
            .Take(pageSize);

        var pageResults = source.ToList();
        
        var result = new BusinessPageResult<TRequest, TId, TEntity>
        {
            Context = pageRequest,
            TotalCount = Convert.ToUInt64(totalCount),
            Result = pageResults,
        };

        return result;
    }
}
