using System.Linq.Expressions;

namespace Saorsa.QueryEngine;

public static class ExpressionBuilder
{
    public static Expression<Func<TEntity, bool>> PropertyEqualTo<TEntity>(
        string propertyName,
        object eqValue)
    {     
        var parameter = Expression.Parameter(typeof(TEntity));
        var parameterProperty = Expression.Property(parameter, propertyName);
        var expectedEquals = Expression.Constant(eqValue);
        var expression = Expression.Equal(parameterProperty, expectedEquals);
        return Expression.Lambda<Func<TEntity, bool>>(expression, parameter);
    }
    
    public static Expression<Func<TEntity, bool>> PropertyNotEqualTo<TEntity>(
        string propertyName,
        object eqValue)
    {     
        var parameter = Expression.Parameter(typeof(TEntity));
        var parameterProperty = Expression.Property(parameter, propertyName);
        var expectedEquals = Expression.Constant(eqValue);
        var expression = Expression.NotEqual(parameterProperty, expectedEquals);
        return Expression.Lambda<Func<TEntity, bool>>(expression, parameter);
    }
    
    public static Expression<Func<TEntity, bool>> PropertyLessThan<TEntity>(
        string propertyName,
        object eqValue)
    {     
        var parameter = Expression.Parameter(typeof(TEntity));
        var parameterProperty = Expression.Property(parameter, propertyName);
        var expectedEquals = Expression.Constant(eqValue);
        var expression = Expression.LessThan(parameterProperty, expectedEquals);
        return Expression.Lambda<Func<TEntity, bool>>(expression, parameter);
    }
    
    public static Expression<Func<TEntity, bool>> PropertyLessThanOrEqual<TEntity>(
        string propertyName,
        object eqValue)
    {     
        var parameter = Expression.Parameter(typeof(TEntity));
        var parameterProperty = Expression.Property(parameter, propertyName);
        var expectedEquals = Expression.Constant(eqValue);
        var expression = Expression.LessThanOrEqual(parameterProperty, expectedEquals);
        return Expression.Lambda<Func<TEntity, bool>>(expression, parameter);
    }
    
    public static Expression<Func<TEntity, bool>> PropertyGreaterThan<TEntity>(
        string propertyName,
        object eqValue)
    {     
        var parameter = Expression.Parameter(typeof(TEntity));
        var parameterProperty = Expression.Property(parameter, propertyName);
        var expectedEquals = Expression.Constant(eqValue);
        var expression = Expression.GreaterThan(parameterProperty, expectedEquals);
        return Expression.Lambda<Func<TEntity, bool>>(expression, parameter);
    }
    
    public static Expression<Func<TEntity, bool>> PropertyGreaterThanOrEqual<TEntity>(
        string propertyName,
        object eqValue)
    {     
        var parameter = Expression.Parameter(typeof(TEntity));
        var parameterProperty = Expression.Property(parameter, propertyName);
        var expectedEquals = Expression.Constant(eqValue);
        
        var expression = Expression.GreaterThanOrEqual(parameterProperty, expectedEquals);
        return Expression.Lambda<Func<TEntity, bool>>(expression, parameter);
    }
}
