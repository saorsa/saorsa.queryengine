using System.Collections;
using System.Linq.Expressions;
using Saorsa.Model;
using Saorsa.QueryEngine.Model;

namespace Saorsa.QueryEngine;

public static class QueryableExtensions
{
    /// <summary>
    /// Creates an expression lambda that returns the value of an entity property at runtime.
    /// </summary>
    /// <param name="propertyName">The property belonging to the TEntity class.</param>
    /// <typeparam name="TEntity">The type of the entity the property belongs to.</typeparam>
    public static Expression<Func<TEntity, object>> ToLambda<TEntity>(string propertyName)
    {
        ExpressionBuilder.CreateParameterPropertyExpression<TEntity>(propertyName);
        var parameter = Expression.Parameter(typeof(TEntity));
        var property = Expression.Property(parameter, propertyName);
        var propAsObject = Expression.Convert(property, typeof(object));

        return Expression.Lambda<Func<TEntity, object>>(propAsObject, parameter);
    }

    public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string propertyName)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "type");
        var propertyExpression = Expression.Property(parameter, propertyName);

        if (propertyExpression.Type != typeof(AutocompleteResponseModel))
            return typeof(IEnumerable).IsAssignableFrom(propertyExpression.Type)
                ? source.OrderBy(propertyExpression.ToCountExpression<TEntity>())
                : source.OrderBy(ToLambda<TEntity>(propertyName));
        var property = parameter.GetPropertyExpression($"{propertyName}.Key");
        return source.OrderBy(Expression.Lambda<Func<TEntity, object>>(property, parameter));
    }

    public static IOrderedQueryable<TEntity> OrderByDescending<TEntity>(this IQueryable<TEntity> source, string propertyName)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "type");
        var propertyExpression = Expression.Property(parameter, propertyName);

        if (propertyExpression.Type != typeof(AutocompleteResponseModel))
            return typeof(IEnumerable).IsAssignableFrom(propertyExpression.Type)
                ? source.OrderByDescending(propertyExpression.ToCountExpression<TEntity>())
                : source.OrderByDescending(ToLambda<TEntity>(propertyName));
        var property = parameter.GetPropertyExpression($"{propertyName}.Key");
        return source.OrderByDescending(Expression.Lambda<Func<TEntity, object>>(property, parameter));
    }
    
    public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, IEnumerable<SortDescriptor>? sortDescriptors)
    {
        if (sortDescriptors == null || !sortDescriptors.Any())
        {
            return source;
        }

        foreach (var sd in sortDescriptors)
        {
            var isDescending = sd.IsDescending.GetValueOrDefault();
            source = isDescending
                ? source.OrderByDescending(sd.Property)
                : source.OrderBy(sd.Property);
        }

        return source;
    }

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
