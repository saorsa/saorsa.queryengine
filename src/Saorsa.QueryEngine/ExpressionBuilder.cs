using System.Linq.Expressions;
using System.Reflection;
using Saorsa.QueryEngine.Model;

namespace Saorsa.QueryEngine;

public static class ExpressionBuilder
{
    public static readonly ConstantExpression NullConstant = Expression.Constant(null);
    
    public static Expression<Func<TEntity, bool>> PropertyEqualTo<TEntity>(
        string propertyName,
        object? argument)
    {
        if (argument == null)
        {
            return PropertyIsNull<TEntity>(propertyName);
        }
        return BuildComparison<TEntity>(
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
        return BuildComparison<TEntity>(
            propertyName, argument, Expression.NotEqual);
    }
    
    public static Expression<Func<TEntity, bool>> PropertyLessThan<TEntity>(
        string propertyName,
        object argument)
    {
        return BuildComparison<TEntity>(
            propertyName, argument, Expression.LessThan);
    }
    
    public static Expression<Func<TEntity, bool>> PropertyLessThanOrEqual<TEntity>(
        string propertyName,
        object argument)
    {
        return BuildComparison<TEntity>(
            propertyName, argument, Expression.LessThanOrEqual);
    }
    
    public static Expression<Func<TEntity, bool>> PropertyGreaterThan<TEntity>(
        string propertyName,
        object argument)
    {
        return BuildComparison<TEntity>(
            propertyName, argument, Expression.GreaterThan);
    }
    
    public static Expression<Func<TEntity, bool>> PropertyGreaterThanOrEqual<TEntity>(
        string propertyName,
        object argument)
    {
        return BuildComparison<TEntity>(
            propertyName, argument, Expression.GreaterThanOrEqual);
    }
    
    public static Expression<Func<TParam, bool>> PropertyIsNull<TParam>(
        string propertyName,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TParam));
        var expression = PropertyIsNullExpression<TParam>(
            propertyName,
            parameter);
        return Expression.Lambda<Func<TParam, bool>>(expression, parameter);
    }

    public static BinaryExpression PropertyIsNullExpression<TParam>(
        string propertyName,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TParam));
        var expression = BuildNullEqualityExpression<TParam>(
            propertyName,
            Expression.Equal,
            parameter);
        return expression;
    }

    
    public static Expression<Func<TParam, bool>> PropertyIsNotNull<TParam>(
        string propertyName,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TParam));
        var expression = PropertyIsNotNullExpression<TParam>(
            propertyName,
            parameter);
        return Expression.Lambda<Func<TParam, bool>>(expression, parameter);
    }

    public static BinaryExpression PropertyIsNotNullExpression<TParam>(
        string propertyName,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TParam));
        var expression = BuildNullEqualityExpression<TParam>(
            propertyName,
            Expression.NotEqual,
            parameter);
        return expression;
    }
    
    public static BinaryExpression BuildNullEqualityExpression<TParam>(
        string propertyName,
        Func<Expression, Expression, BinaryExpression> nullEqualityFunc,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TParam));
        var parameterProperty = Expression.Property(parameter, propertyName);
        var expression = nullEqualityFunc(parameterProperty, NullConstant);
        return expression;
    }
    
    public static Expression<Func<TParam, bool>> BuildComparison<TParam>(
        string propertyName,
        object argument,
        Func<Expression, Expression, BinaryExpression> binaryExpressionBuilder,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TParam));
        var expression = BuildComparisonExpression<TParam>(
            propertyName,
            argument,
            binaryExpressionBuilder,
            parameter);
        return Expression.Lambda<Func<TParam, bool>>(expression, parameter);
    }
    
    public static BinaryExpression BuildComparisonExpression<TParam>(
        string propertyName,
        object argument,
        Func<Expression, Expression, BinaryExpression> compareFunc,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TParam));
        var argumentType = argument.GetType();
        var parameterProperty = Expression.Property(parameter, propertyName);
        var parameterPropertyType = ((PropertyInfo)parameterProperty.Member).PropertyType;

        if (argumentType == parameterPropertyType)
        {
            var argumentConstant = Expression.Constant(argument);
            var directExpression = compareFunc(parameterProperty, argumentConstant);
            return directExpression;
        }

        var expectedConvertedConstant = Expression.Convert(
            Expression.Constant(argument),
            parameterPropertyType
        );
        
        var conversionExpression = compareFunc(parameterProperty, expectedConvertedConstant);
        return conversionExpression;
    }
}
