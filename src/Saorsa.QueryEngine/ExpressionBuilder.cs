using System.Linq.Expressions;
using System.Reflection;

namespace Saorsa.QueryEngine;

public static class ExpressionBuilder
{
    public static readonly ConstantExpression NullConstant = Expression.Constant(null);

    public static Expression<Func<TParam, bool>> PropertyEqualTo<TParam>(
        string propertyName,
        object? argument,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TParam));
        var expression = PropertyEqualToExpression<TParam>(
            propertyName,
            argument,
            parameter);
        return Expression.Lambda<Func<TParam, bool>>(expression, parameter);
    }

    public static BinaryExpression PropertyEqualToExpression<TParam>(
        string propertyName,
        object? argument,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TParam));
        if (argument == null)
        {
            return PropertyIsNullExpression<TParam>(propertyName, parameter);
        }
        return BuildComparisonExpression<TParam>(
            propertyName,
            argument,
            Expression.Equal,
            parameter);
    }

    public static Expression<Func<TParam, bool>> PropertyNotEqualTo<TParam>(
        string propertyName,
        object? argument,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TParam));
        var expression = PropertyNotEqualToExpression<TParam>(
            propertyName,
            argument,
            parameter);
        return Expression.Lambda<Func<TParam, bool>>(expression, parameter);
    }

    public static BinaryExpression PropertyNotEqualToExpression<TParam>(
        string propertyName,
        object? argument,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TParam));
        if (argument == null)
        {
            return PropertyIsNotNullExpression<TParam>(propertyName, parameter);
        }
        return BuildComparisonExpression<TParam>(
            propertyName,
            argument,
            Expression.NotEqual,
            parameter);
    }

    public static Expression<Func<TParam, bool>> PropertyLessThan<TParam>(
        string propertyName,
        object argument,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TParam));
        var expression = PropertyLessThanExpression<TParam>(
            propertyName,
            argument,
            parameter);
        return Expression.Lambda<Func<TParam, bool>>(expression, parameter);
    }

    public static BinaryExpression PropertyLessThanExpression<TParam>(
        string propertyName,
        object argument,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TParam));
        return BuildComparisonExpression<TParam>(
            propertyName,
            argument,
            Expression.LessThan,
            parameter);
    }

    public static Expression<Func<TParam, bool>> PropertyLessThanOrEqual<TParam>(
        string propertyName,
        object argument,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TParam));
        var expression = PropertyLessThanOrEqualExpression<TParam>(
            propertyName,
            argument,
            parameter);
        return Expression.Lambda<Func<TParam, bool>>(expression, parameter);
    }

    public static BinaryExpression PropertyLessThanOrEqualExpression<TParam>(
        string propertyName,
        object argument,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TParam));
        return BuildComparisonExpression<TParam>(
            propertyName,
            argument,
            Expression.LessThanOrEqual,
            parameter);
    }

    public static Expression<Func<TParam, bool>> PropertyGreaterThan<TParam>(
        string propertyName,
        object argument,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TParam));
        var expression = PropertyGreaterThanExpression<TParam>(
            propertyName,
            argument,
            parameter);
        return Expression.Lambda<Func<TParam, bool>>(expression, parameter);
    }

    public static BinaryExpression PropertyGreaterThanExpression<TParam>(
        string propertyName,
        object argument,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TParam));
        return BuildComparisonExpression<TParam>(
            propertyName,
            argument,
            Expression.GreaterThan,
            parameter);
    }

    public static Expression<Func<TParam, bool>> PropertyGreaterThanOrEqual<TParam>(
        string propertyName,
        object argument,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TParam));
        var expression = PropertyGreaterThanOrEqualExpression<TParam>(
            propertyName,
            argument,
            parameter);
        return Expression.Lambda<Func<TParam, bool>>(expression, parameter);
    }

    public static BinaryExpression PropertyGreaterThanOrEqualExpression<TParam>(
        string propertyName,
        object argument,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TParam));
        return BuildComparisonExpression<TParam>(
            propertyName,
            argument,
            Expression.GreaterThanOrEqual,
            parameter);
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
