using System.Linq.Expressions;

namespace Saorsa.QueryEngine;

public static class ExpressionExtensions
{
    public static Expression AggregateNestedProperty(
        this Expression parameter,
        string property,
        string separator = ".")
    {
        var nestedProperties = property.Split(separator);
        if (nestedProperties.Length != 2)
        {
            throw new ArgumentException(
                $"Property '{property}' does not conform to the pattern " 
                + $"[entityType]{separator}[propertyName].");
        }
        
        return nestedProperties.Aggregate(parameter, Expression.PropertyOrField);
    }

    public static Expression<Func<TEntity, int>> ToCountExpression<TEntity>(
        this Expression propertyExpression,
        ParameterExpression parameter)
    {
        var baseType = propertyExpression.Type.GetGenericArguments()[0];
        var enumerableCountMethod = typeof(Enumerable).GetMethods()
            .First(method => method.Name == "Count" && method.GetParameters().Length == 1)
            .MakeGenericMethod(baseType);
        var expression = Expression.Call(enumerableCountMethod, propertyExpression);
        return Expression.Lambda<Func<TEntity, int>>(expression, parameter);
    }
}