using System.Linq.Expressions;
using System.Reflection;
using Saorsa.QueryEngine.Model;

namespace Saorsa.QueryEngine;

public static class ExpressionBuilder
{
    public static Expression<Func<TEntity, bool>> BuildPropertyBinaryExpression<TEntity>(
        string propertyName,
        object argument,
        Func<Expression, Expression, BinaryExpression> binaryExpressionBuilder)
    {
        var argumentType = argument.GetType();
        var parameter = Expression.Parameter(typeof(TEntity));
        var parameterProperty = Expression.Property(parameter, propertyName);
        var parameterPropertyType = ((PropertyInfo)parameterProperty.Member).PropertyType;

        if (argumentType == parameterPropertyType)
        {
            var argumentConstant = Expression.Constant(argument);
            var directExpression = binaryExpressionBuilder(parameterProperty, argumentConstant);
            return Expression.Lambda<Func<TEntity, bool>>(directExpression, parameter);
        }

        var expectedConvertedConstant = Expression.Convert(
            Expression.Constant(argument),
            parameterPropertyType
        );
        
        var conversionExpression = binaryExpressionBuilder(parameterProperty, expectedConvertedConstant);
        return Expression.Lambda<Func<TEntity, bool>>(conversionExpression, parameter);
    }
    
    public static Expression<Func<TEntity, bool>> PropertyEqualTo<TEntity>(
        string propertyName,
        object? argument)
    {
        if (argument == null)
        {
            return PropertyIsNull<TEntity>(propertyName);
        }
        return BuildPropertyBinaryExpression<TEntity>(
            propertyName, argument, Expression.Equal);
    }
    
    public static Expression<Func<TEntity, bool>> PropertyNotEqualTo<TEntity>(
        string propertyName,
        object? argument)
    {
        if (argument == null)
        {
            return PropertyIsNotNull<TEntity>(propertyName);
        }
        return BuildPropertyBinaryExpression<TEntity>(
            propertyName, argument, Expression.NotEqual);
    }
    
    public static Expression<Func<TEntity, bool>> PropertyLessThan<TEntity>(
        string propertyName,
        object argument)
    {
        return BuildPropertyBinaryExpression<TEntity>(
            propertyName, argument, Expression.LessThan);
    }
    
    public static Expression<Func<TEntity, bool>> PropertyLessThanOrEqual<TEntity>(
        string propertyName,
        object argument)
    {
        return BuildPropertyBinaryExpression<TEntity>(
            propertyName, argument, Expression.LessThanOrEqual);
    }
    
    public static Expression<Func<TEntity, bool>> PropertyGreaterThan<TEntity>(
        string propertyName,
        object argument)
    {
        return BuildPropertyBinaryExpression<TEntity>(
            propertyName, argument, Expression.GreaterThan);
    }
    
    public static Expression<Func<TEntity, bool>> PropertyGreaterThanOrEqual<TEntity>(
        string propertyName,
        object argument)
    {
        return BuildPropertyBinaryExpression<TEntity>(
            propertyName, argument, Expression.GreaterThanOrEqual);
    }
    
    public static Expression<Func<TEntity, bool>> PropertyIsNull<TEntity>(
        string propertyName)
    {     
        var parameter = Expression.Parameter(typeof(TEntity));
        var parameterProperty = Expression.Property(parameter, propertyName);
        var expectedEquals = Expression.Constant(null);
        var expression = Expression.Equal(parameterProperty, expectedEquals);
        return Expression.Lambda<Func<TEntity, bool>>(expression, parameter);
    }
    
    public static Expression<Func<TEntity, bool>> PropertyIsNotNull<TEntity>(
        string propertyName)
    {     
        var parameter = Expression.Parameter(typeof(TEntity));
        var parameterProperty = Expression.Property(parameter, propertyName);
        var expectedEquals = Expression.Constant(null);
        var expression = Expression.NotEqual(parameterProperty, expectedEquals);
        return Expression.Lambda<Func<TEntity, bool>>(expression, parameter);
    }
}
