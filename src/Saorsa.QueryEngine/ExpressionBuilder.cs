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

        var convertedVal = argument.ConvertQueryEngineType(parameterPropertyType);
        
        var expectedConvertedConstant = Expression.Convert(
            Expression.Constant(convertedVal),
            parameterPropertyType
        );
        
        var conversionExpression = compareFunc(parameterProperty, expectedConvertedConstant);
        return conversionExpression;
    }

    public static object? ConvertQueryEngineType(
        this object? source,
        Type targetType)
    {
        
        var isValueType = targetType.IsValueType;
        var isNullable = targetType.IsNullable();
        var required = isValueType && !isNullable;
        targetType = targetType.GetUnderlyingTypeIfNullable();
        
        if (source == null)
        {
            throw new QueryEngineException(
                ErrorCodes.TypeConversionError,
                $"Value of type '{targetType.Name}' is required, but provided value is undefined.");
        }
        
        if (source is JsonElement json)
        {
            return json.ConvertQueryEngineType(targetType.GetUnderlyingTypeIfNullable(), required);
        }
        
        if (targetType == typeof(char))
        {
            return source is char v ? v : Convert.ToChar(source);
        }
        if (targetType == typeof(string))
        {
            return source as string ?? Convert.ToString(source);
        }
        if (targetType == typeof(bool))
        {
            return source is bool v ? v : Convert.ToBoolean(source);
        }
        if (targetType == typeof(byte))
        {
            return source is byte v ? v : Convert.ToByte(source);
        }
        if (targetType == typeof(sbyte))
        {
            return source is sbyte v ? v : Convert.ToSByte(source);
        }
        if (targetType == typeof(short))
        {
            return source is short v ? v : Convert.ToInt16(source);
        }
        if (targetType == typeof(int))
        {
            return source is int v ? v : Convert.ToInt32(source);
        }
        if (targetType == typeof(long))
        {
            return source is int v ? v : Convert.ToInt64(source);
        }
        if (targetType == typeof(ushort))
        {
            return source is ushort v ? v : Convert.ToUInt16(source);
        }
        if (targetType == typeof(uint))
        {
            return source is uint v ? v : Convert.ToUInt32(source);
        }
        if (targetType == typeof(ulong))
        {
            return source is ulong v ? v : Convert.ToUInt64(source);
        }
        if (targetType == typeof(float))
        {
            return source is float v ? v : Convert.ToSingle(source);
        }
        if (targetType == typeof(double))
        {
            return source is double v ? v : Convert.ToDouble(source);
        }
        if (targetType == typeof(decimal))
        {
            return source is decimal v ? v : Convert.ToDecimal(source);
        }
        if (targetType == typeof(Guid))
        {
            return source is Guid v ? v : Guid.Parse(source.ToString()!);
        }
        if (targetType == typeof(DateTime))
        {
            return source is DateTime v ? v : DateTime.Parse(source.ToString()!);
        }
        if (targetType == typeof(DateOnly))
        {
            return source is DateOnly v ? v : DateOnly.Parse(source.ToString()!);
        }
        if (targetType == typeof(DateTimeOffset))
        {
            return source is DateTimeOffset v ? v : DateTimeOffset.Parse(source.ToString()!);
        }
        if (targetType == typeof(TimeSpan))
        {
            return source is TimeSpan v ? v : TimeSpan.Parse(source.ToString()!);
        }
        if (targetType == typeof(TimeOnly))
        {
            return source is TimeOnly v ? v : TimeOnly.Parse(source.ToString()!);
        }
        
        throw new QueryEngineException(
            ErrorCodes.TypeConversionError,
            $"Value '{source.ToString()}' cannot be represented as '{targetType.Name}'");
    }
    
    public static object? ConvertQueryEngineType(
        this JsonElement source,
        Type targetType,
        bool required = true)
    {
        switch (source.ValueKind)
        {
            case JsonValueKind.Undefined:
                if (required)
                {
                    throw new QueryEngineException(
                        ErrorCodes.TypeConversionError,
                        $"Value of type '{targetType.Name}' is required, but provided value is undefined.");
                }
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

            case JsonValueKind.Object:
                throw new QueryEngineException(
                    ErrorCodes.TypeConversionLiteralValueExpectedError,
                    $"Value of type '{targetType.Name}' cannot be converted from JsonElement '{source.ToString()}'.");
            case JsonValueKind.Array:
                throw new QueryEngineException(
                    ErrorCodes.TypeConversionLiteralValueExpectedError,
                    $"Value of type '{targetType.Name}' cannot be converted from JsonElement '{source.ToString()}'.");
            case JsonValueKind.String:
            {
                
                var stringVal = source.GetString()!;
                if (targetType == typeof(char))
                {
                    return Convert.ToChar(stringVal);
                }
                if (targetType == typeof(string))
                {
                    return stringVal;
                }
                if (targetType == typeof(bool))
                {
                    return Convert.ToBoolean(stringVal);
                }
                if (targetType == typeof(byte))
                {
                    return Convert.ToByte(stringVal);
                }
                if (targetType == typeof(sbyte))
                {
                    return Convert.ToSByte(stringVal);
                }
                if (targetType == typeof(short))
                {
                    return Convert.ToInt16(stringVal);
                }
                if (targetType == typeof(int))
                {
                    return Convert.ToInt32(stringVal);
                }
                if (targetType == typeof(long))
                {
                    return Convert.ToInt64(stringVal);
                }
                if (targetType == typeof(ushort))
                {
                    return Convert.ToInt16(stringVal);
                }
                if (targetType == typeof(uint))
                {
                    return Convert.ToUInt32(stringVal);
                }
                if (targetType == typeof(ulong))
                {
                    return Convert.ToUInt64(stringVal);
                }
                if (targetType == typeof(float))
                {
                    return Convert.ToSingle(stringVal);
                }
                if (targetType == typeof(double))
                {
                    return Convert.ToDouble(stringVal);
                }
                if (targetType == typeof(decimal))
                {
                    return Convert.ToDecimal(stringVal);
                }
                if (targetType == typeof(Guid))
                {
                    return Guid.Parse(stringVal);
                }
                if (targetType == typeof(DateTime))
                {
                    return DateTime.Parse(stringVal, CultureInfo.InvariantCulture);
                }
                if (targetType == typeof(DateOnly))
                {
                    return DateOnly.Parse(stringVal, CultureInfo.InvariantCulture);
                }
                if (targetType == typeof(DateTimeOffset))
                {
                    return DateTimeOffset.Parse(stringVal, CultureInfo.InvariantCulture);
                }
                if (targetType == typeof(TimeSpan))
                {
                    return TimeSpan.Parse(stringVal, CultureInfo.InvariantCulture);
                }
                if (targetType == typeof(TimeOnly))
                {
                    return TimeOnly.Parse(stringVal, CultureInfo.InvariantCulture);
                }
                
                throw new QueryEngineException(
                    ErrorCodes.TypeConversionError,
                    $"Value '{source.ToString()}' cannot be represented as '{targetType.Name}'");
            }
            case JsonValueKind.Number:
                if (targetType == typeof(char))
                {
                    var val = source.GetDecimal();
                    return Convert.ToChar(val);
                }
                if (targetType == typeof(string))
                {
                    return source.GetString()!;
                }
                if (targetType == typeof(bool))
                {
                    return source.GetBoolean();
                }
                if (targetType == typeof(byte))
                {
                    return source.GetByte();
                }
                if (targetType == typeof(sbyte))
                {
                    return source.GetSByte();
                }
                if (targetType == typeof(short))
                {
                    return source.GetInt16();
                }
                if (targetType == typeof(int))
                {
                    return source.GetInt32();
                }
                if (targetType == typeof(long))
                {
                    return source.GetInt64();
                }
                if (targetType == typeof(ushort))
                {
                    return source.GetUInt16();
                }
                if (targetType == typeof(uint))
                {
                    return source.GetUInt32();
                }
                if (targetType == typeof(ulong))
                {
                    return source.GetUInt64();
                }
                if (targetType == typeof(float))
                {
                    return source.GetSingle();
                }
                if (targetType == typeof(double))
                {
                    return source.GetDouble();
                }
                if (targetType == typeof(decimal))
                {
                    return source.GetDecimal();
                }
                if (targetType == typeof(Guid))
                {
                    return source.GetGuid();
                }
                if (targetType == typeof(DateTime))
                {
                    var stringVal = source.GetString();
                    return DateTime.Parse(stringVal!, CultureInfo.InvariantCulture);
                }
                if (targetType == typeof(DateOnly))
                {
                    var stringVal = source.GetString();
                    return DateOnly.Parse(stringVal!, CultureInfo.InvariantCulture);
                }
                if (targetType == typeof(DateTimeOffset))
                {
                    var stringVal = source.GetString();
                    return DateTimeOffset.Parse(stringVal!, CultureInfo.InvariantCulture);
                }
                if (targetType == typeof(TimeSpan))
                {
                    var stringVal = source.GetString();
                    return TimeSpan.Parse(stringVal!, CultureInfo.InvariantCulture);
                }
                if (targetType == typeof(TimeOnly))
                {
                    var stringVal = source.GetString();
                    return TimeOnly.Parse(stringVal!, CultureInfo.InvariantCulture);
                }
                
                throw new QueryEngineException(
                    ErrorCodes.TypeConversionError,
                    $"Value '{source.ToString()}' cannot be represented as '{targetType.Name}'");
                
            case JsonValueKind.True:
                if (targetType == typeof(bool))
                {
                    return source.GetBoolean();
                }
                throw new QueryEngineException(
                    ErrorCodes.TypeConversionError,
                    $"Value '{source.ToString()}' cannot be represented as '{targetType.Name}'");
            case JsonValueKind.False:
                if (targetType == typeof(bool))
                {
                    return source.GetBoolean();
                }
                throw new QueryEngineException(
                    ErrorCodes.TypeConversionError,
                    $"Value '{source.ToString()}' cannot be represented as '{targetType.Name}'");
            case JsonValueKind.Null:
                
                if (required)
                {
                    throw new QueryEngineException(
                        ErrorCodes.TypeConversionError,
                        $"Value of type '{targetType.Name}' is required, but provided value is null.");
                }
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            default:
                throw new NotSupportedException($"JsonElement of type '{source.ValueKind}' is not supported.");
        }
    }
}
