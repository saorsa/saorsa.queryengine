using System.Linq.Expressions;
using System.Reflection;

namespace Saorsa.QueryEngine;


/// <summary>
/// Represents an utility / helper class for <see cref="Т:System.Linq.Expressions.Expression"/> and derived
/// expression types creation.
/// </summary>
public static class ExpressionBuilder
{
    /// <summary>
    /// The default expression matching a NULL constant.
    /// </summary>
    public static readonly ConstantExpression NullObjectConstant = Expression.Constant(null);

    /// <summary>
    /// Creates an expression lambda that returns the value of an entity property at runtime.
    /// </summary>
    /// <param name="propertyName">The property belonging to the TEntity class.</param>
    /// <typeparam name="TEntity">The type of the entity the property belongs to.</typeparam>
    /// <typeparam name="TProperty">
    /// The type of the property associated with the <paramref name="propertyName"/>.
    /// </typeparam>
    public static Expression<Func<TEntity, TProperty>> GetPropertyAccessorExpression<TEntity, TProperty>(
        string propertyName)
    {
        var parameterType = typeof(TEntity);
        var parameter = Expression.Parameter(
            parameterType, 
            $"Param_ToLambda<{parameterType.Name},{typeof(TProperty).Name}>");

        var property = parameter.GetPropertyExpression(propertyName);
        var propAsObject = Expression.Convert(property, typeof(TProperty));

        return Expression.Lambda<Func<TEntity, TProperty>>(propAsObject, parameter);
    }

    /// <summary>
    /// Creates an expression lambda that returns the value of an entity property at runtime.
    /// </summary>
    /// <param name="propertyName">The property belonging to the TEntity class.</param>
    /// <typeparam name="TEntity">The type of the entity the property belongs to.</typeparam>
    public static Expression<Func<TEntity, object>> GetPropertyAccessorExpression<TEntity>(
        string propertyName)
    {
        return GetPropertyAccessorExpression<TEntity, object>(propertyName);
    }

    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MemberExpression" /> that represents accessing a property
    /// of an object instance of the type specified by <typeparamref name="TParam"/>.
    /// </summary>
    /// <param name="propertyName">
    /// The name of a property to be accessed.
    /// </param>
    /// <param name="existingParameter">
    /// An <see cref="T:System.Linq.Expressions.Expression" /> whose
    /// <see cref="P:System.Linq.Expressions.Expression.Type" /> contains a property named
    /// <paramref name="propertyName" />. If not specified, this parameter will be automatically created.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// Thrown if <paramref name="propertyName" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="T:System.ArgumentException">
    /// Thrown if no property named <paramref name="propertyName" /> is defined in <typeparamref name="TParam" />.
    /// type or its base types --or-- if the <paramref name="existingParameter" /> parameter is defined and matches
    /// a different type than the <typeparamref name="TParam"/>.
    /// </exception>
    public static MemberExpression CreateParameterPropertyExpression<TParam>(
        string propertyName,
        ParameterExpression? existingParameter = null)
    {
        return CreateParameterPropertyExpression(typeof(TParam), propertyName, existingParameter);
    }
    
    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MemberExpression" /> that represents accessing a property
    /// of an object instance of the type specified by <paramdef name="parameterType"/>.
    /// </summary>
    /// <param name="parameterType">
    /// The type of the source parameter.
    /// </param>
    /// <param name="propertyName">
    /// The name of a property to be accessed.
    /// </param>
    /// <param name="existingParameter">
    /// An <see cref="T:System.Linq.Expressions.Expression" /> whose
    /// <see cref="P:System.Linq.Expressions.Expression.Type" /> contains a property named
    /// <paramref name="propertyName" />. If not specified, this parameter will be automatically created.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// Thrown if <paramref name="propertyName" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="T:System.ArgumentException">
    /// Thrown if no property named <paramref name="propertyName" /> is defined in <paramdef name="parameterType" />.
    /// type or its base types --or-- if the <paramref name="existingParameter" /> parameter is defined and matches
    /// a different type than the <paramref name="parameterType"/>.
    /// </exception>
    public static MemberExpression CreateParameterPropertyExpression(
        Type parameterType,
        string propertyName,
        ParameterExpression? existingParameter = null)
    {
        ValidateParameterExpressionType(parameterType, existingParameter);

        var parameter = existingParameter ?? Expression.Parameter(
            parameterType, 
            $"Param_CreateParameterPropertyExpression<{parameterType.Name}>");
        return Expression.Property(parameter, propertyName);
    }

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
        var parameterProperty = parameter.GetPropertyExpression(propertyName);
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

    /// <summary>
    /// Checks if a given parameter expression has a type matching an expected type. Throws exception if
    /// validation fails.
    /// </summary>
    /// <param name="parameterType"></param>
    /// <param name="existingParameter"></param>
    /// <exception cref="ArgumentException">
    /// Thrown, if the type specified by <paramref name="parameterType"/> does not match the type of the parameter
    /// expression defined in <paramref name="existingParameter"/>.
    /// </exception>
    public static void ValidateParameterExpressionType(
        Type parameterType,
        ParameterExpression? existingParameter)
    {
        if (existingParameter != null && existingParameter.Type != parameterType)
        {
            throw new ArgumentException(
                $"The existingParameter argument has underlying type [{existingParameter.Type}] which is " +
                $"different than the one specified by the parameterType argument [{parameterType}].",
                nameof(existingParameter));
        }
    }
}
