using System.Linq.Expressions;
using Saorsa.QueryEngine.Model;

namespace Saorsa.QueryEngine;

public static class QueryableExtensions
{
    public static IOrderedQueryable<TEntity> OrderBy<TEntity>(
        this IQueryable<TEntity> source,
        string propertyName)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "type");
        var propertyExpression = Expression.Property(parameter, propertyName);

        if (propertyExpression.Type != typeof(AutocompleteResponseModel))
            return propertyExpression.Type.IsEnumerable()
        //IsEnumerable(propertyExpression.Type.FullName)
                ? source.OrderBy(propertyExpression.ToCountExpression<TEntity>(parameter))
                : source.OrderBy( QueryEngineUtil.ToLambda<TEntity>(propertyName));
        var property = parameter.AggregateNestedProperty(
            $"{propertyName}.{nameof(AutocompleteResponseModel.Key)}");
        return source.OrderBy(Expression.Lambda<Func<TEntity, object>>(
            property,
            parameter));
    }
    
    public static IOrderedQueryable<TEntity> OrderByDescending<TEntity>(
        this IQueryable<TEntity> source,
        string propertyName)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "type");
        var propertyExpression = Expression.Property(parameter, propertyName);

        if (propertyExpression.Type != typeof(AutocompleteResponseModel))
            return propertyExpression.Type.IsEnumerable()
                //IsEnumerable(propertyExpression.Type.FullName)
                ? source.OrderByDescending(propertyExpression.ToCountExpression<TEntity>(parameter))
                : source.OrderByDescending( QueryEngineUtil.ToLambda<TEntity>(propertyName));
        var property = parameter.AggregateNestedProperty(
            $"{propertyName}.{nameof(AutocompleteResponseModel.Key)}");
        return source.OrderByDescending(Expression.Lambda<Func<TEntity, object>>(
            property,
            parameter));
    }

}
