using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace Saorsa.QueryEngine;

public static class ExpressionBuilder
{
    public static readonly ConstantExpression NullObjectConstant = Expression.Constant(null);

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
        if (parameterProperty.Type.IsValueType)
        {
            var defaultValue = Activator.CreateInstance(parameterProperty.Type);
            var expression2 = nullEqualityFunc(parameterProperty, Expression.Constant(defaultValue));
            return expression2;
        }
        var expression = nullEqualityFunc(parameterProperty, NullObjectConstant);
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

        var convertedVal = argument.ConvertToAtom(parameterPropertyType);
        
        var expectedConvertedConstant = Expression.Convert(
            Expression.Constant(convertedVal),
            parameterPropertyType
        );
        
        var conversionExpression = compareFunc(parameterProperty, expectedConvertedConstant);
        return conversionExpression;
    }
    
    public static BinaryExpression BuildContainsExpression<TParam>(
        string propertyName,
        object argument,
        ParameterExpression? existingParameter = null)
    {
        /**
         * 
        var baseType = propertyExpression.Type.GetGenericArguments()[0];
        var enumerableCountMethod = typeof(Enumerable).GetMethods()
            .First(method => method.Name == "Count" && method.GetParameters().Length == 1)
            .MakeGenericMethod(baseType);
        var expression = Expression.Call(enumerableCountMethod, propertyExpression);
        return Expression.Lambda<Func<TEntity, int>>(expression, parameter);
         */
        
        var parameter = existingParameter ?? Expression.Parameter(typeof(TParam));
        var argumentType = argument.GetType();
        var parameterProperty = Expression.Property(parameter, propertyName);
        var parameterPropertyType = ((PropertyInfo)parameterProperty.Member).PropertyType;
        var convertedVal = argument.ConvertToAtom(parameterPropertyType);

        if (convertedVal != null)
        {
            var argumentConstant = Expression.Constant(convertedVal);
            
            var truth = Expression.Constant(true);

            var typeContainsMethod = parameterPropertyType.GetMethods()
                .First(method => method.Name == "Contains" && method.GetParameters().Length == 1);

            var containsExpression = Expression.Call(parameterProperty, typeContainsMethod, argumentConstant);
            //var expression = Expression.Call(typeContainsMethod, parameterProperty, argumentConstant);

            var containsExp = Expression.MakeBinary(
                ExpressionType.Equal, containsExpression, truth, false, null);
            return containsExp ;
            
            /**
            return Expression.MakeBinary(ExpressionType.Call, expression, argumentConstant);

            var method = typeof(ExpressionBuilder).GetMethod(
                nameof(StringContains),
                new[] { typeof(string), typeof(string) })!;
            
            var containsExp = Expression.MakeBinary(
                ExpressionType.Equal, parameterProperty, argumentConstant, false, method);
            return containsExp ;*/
        }

        throw new NotSupportedException(
            $"Cannot build a contains expression for parameter of type {parameterPropertyType} " +
            $"and argument of type {argumentType}");
    }

    public static bool StringContains(string source, string contained)
    {
        return source.Contains(contained, StringComparison.OrdinalIgnoreCase);
    }
}
