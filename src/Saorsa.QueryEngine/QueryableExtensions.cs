using System.Collections;
using System.Linq.Expressions;
using Saorsa.Model;
using Saorsa.QueryEngine.Model;

namespace Saorsa.QueryEngine;

public static class QueryableExtensions
{
    /// <summary>Sorts the elements of a sequence in ascending order according to a key.</summary>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="propertyName">
    /// The name of the property belonging to the type defined by <typeparamref name="TEntity"/>.
    /// </param>
    /// <typeparam name="TEntity">The type of the elements of <paramref name="source" />.</typeparam>
    /// <exception cref="T:System.ArgumentNullException">
    /// Thrown if <paramref name="source" />  is <see langword="null" />.
    /// </exception>
    public static IOrderedQueryable<TEntity> OrderBy<TEntity>(
        this IQueryable<TEntity> source,
        string propertyName)
    {
        var parameterType = typeof(TEntity);
        var parameter =  Expression.Parameter(
            parameterType, 
            $"Param_OrderBy<{parameterType.Name}>");
        var propertyExpression = parameter.GetPropertyExpression(propertyName);

        if (propertyExpression.Type != typeof(QueryEngineAutocompleteDescriptor))
            return typeof(IEnumerable).IsAssignableFrom(propertyExpression.Type)
                ? source.OrderBy(propertyExpression.ToCountExpression<TEntity>())
                : source.OrderBy(ExpressionBuilder.GetPropertyAccessorExpression<TEntity>(propertyName));
        var sortProperty = parameter.GetPropertyExpression(
            $"{propertyName}.{nameof(QueryEngineAutocompleteDescriptor.Key)}");
        return source.OrderByDescending(Expression.Lambda<Func<TEntity, object>>(sortProperty, parameter));
    }

    /// <summary>Sorts the elements of a sequence in descending order according to a key.</summary>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="propertyName">
    /// The name of the property belonging to the type defined by <typeparamref name="TEntity"/>.
    /// </param>
    /// <typeparam name="TEntity">The type of the elements of <paramref name="source" />.</typeparam>
    /// <exception cref="T:System.ArgumentNullException">
    /// Thrown if <paramref name="source" />  is <see langword="null" />.
    /// </exception>
    public static IOrderedQueryable<TEntity> OrderByDescending<TEntity>(
        this IQueryable<TEntity> source,
        string propertyName)
    {
        var parameterType = typeof(TEntity);
        var parameter =  Expression.Parameter(
            parameterType, 
            $"Param_OrderByDescending<{parameterType.Name}>");
        var propertyExpression = parameter.GetPropertyExpression(propertyName);

        if (propertyExpression.Type != typeof(QueryEngineAutocompleteDescriptor))
            return typeof(IEnumerable).IsAssignableFrom(propertyExpression.Type)
                ? source.OrderByDescending(propertyExpression.ToCountExpression<TEntity>())
                : source.OrderByDescending(ExpressionBuilder.GetPropertyAccessorExpression<TEntity>(propertyName));
        var sortProperty = parameter.GetPropertyExpression(
            $"{propertyName}.{nameof(QueryEngineAutocompleteDescriptor.Key)}");
        return source.OrderByDescending(Expression.Lambda<Func<TEntity, object>>(sortProperty, parameter));
    }
 
    /// <summary>Sorts the elements of a sequence.</summary>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="sortDescriptors">
    /// The collection of sort descriptors to be used for sort. Sort descriptors are applied one after another in
    /// their original order, provided in the collection.
    /// </param>
    /// <typeparam name="TEntity">The type of the elements of <paramref name="source" />.</typeparam>
    public static IQueryable<TEntity> OrderBy<TEntity>(
        this IQueryable<TEntity> source,
        IEnumerable<SortDescriptor>? sortDescriptors)
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
        FilterPropertyDescriptor filterPropertyDescriptor)
    {
        return QueryEngine.AddPropertyFilter(source, filterPropertyDescriptor);
    }
    
    public static IQueryable<TEntity> Where<TEntity>(
        this IQueryable<TEntity> source,
        FilterBlockDescriptor blockDescriptor)
    {
        return QueryEngine.AddPropertyFilterBlock(source, blockDescriptor);
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
