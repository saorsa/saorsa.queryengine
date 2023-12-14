using System.Linq.Expressions;

namespace Saorsa.QueryEngine;


/// <summary>
/// Extension methods for the <see cref="T:System.Linq.Expressions.Expression" /> and derived types.
/// </summary>
public static class ExpressionExtensions
{
    /// <summary>
    /// The symbol used for sequencing in nested property expressions.
    /// </summary>
    public static readonly string NestedPropertySeparator = ".";
    
    /// <summary>
    /// Creates a <see cref="T:System.Linq.Expressions.MemberExpression" /> that represents accessing a property.
    /// </summary>
    /// <param name="parameter">
    /// An <see cref="T:System.Linq.Expressions.ParameterExpression" /> whose
    /// <see cref="P:System.Linq.Expressions.Expression.Type" /> contains a property named
    /// <paramref name="propertyName" />. This can be <see langword="null" /> for static properties.
    /// </param>
    /// <param name="propertyName">The name of a property to be accessed.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// Thrown if <paramref name="parameter" /> or <paramref name="propertyName" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="T:System.ArgumentException">
    /// Thrown if no property named <paramref name="propertyName" /> is defined in
    /// <paramref name="parameter" /> type or its base types.
    /// </exception>
    public static MemberExpression GetPropertyExpression(
        this ParameterExpression parameter,
        string propertyName)
    {
        if (propertyName.Contains(NestedPropertySeparator))
        {
            var nestedProperties = propertyName.Split(NestedPropertySeparator);
            return (MemberExpression) nestedProperties.Aggregate((Expression)parameter, Expression.PropertyOrField);
        }

        return Expression.Property(parameter, propertyName);
    }

    /// <summary>
    /// Builds an expression that performs an aggregated count against a source property expression. The property 
    /// expression --must-- be an array of elements or generic enumerable. The expression results in a mapping between
    /// a source entity type, containing the property, and the total count of elements in the collection matching that
    /// property.
    /// </summary>
    /// <param name="property">
    /// The source property expression reference. Must be referencing a property of either generic enumerable or
    /// strict array of elements.
    /// </param>
    /// <typeparam name="TEntity">
    /// The type of the source entity, which contains the property.
    /// </typeparam>
    /// <exception cref="Т:System.ArgumentNullException">
    /// Thrown if the source <paramref name="property"/> is NULL.
    /// </exception>
    /// <exception cref="Т:System.ArgumentException">
    /// Thrown under one of the following conditions:
    /// <list type="bullet">
    /// <item>
    ///     The Expression property of the source <paramref name="property"/> is NULL</item>
    /// <item>
    ///     The Expression property of the source <paramref name="property"/> is not a valid
    ///     <seealso cref="T:System.Linq.Expressions.ParameterExpression"/> instance</item>
    /// <item>
    ///     The Expression property of the source <paramref name="property"/> is not of the same type
    ///     as the source <typeparamref name="TEntity"/>.
    /// </item>
    /// </list>
    /// </exception>
    public static Expression<Func<TEntity, int>> ToCountExpression<TEntity>(
        this MemberExpression property)
    {
        if (property == null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        if (property.Expression == null)
        {
            throw new ArgumentException(
                $"The property expression [{property.GetType()}] is not a valid member expression. " +
                $"Its parameter / entity expression property is NULL.",
                nameof(property));
        }

        var parameter = property.Expression!;
        if (parameter is not ParameterExpression parameterExpression)
        {
            throw new ArgumentException(
                $"The property expression [{property.GetType()}] is not a valid member expression. " +
                $"Its expression [{parameter.GetType()}] is not a valid [{typeof(ParameterExpression)}].",
                nameof(property));
        }

        if (parameterExpression.Type != typeof(TEntity))
        {
            throw new ArgumentException(
                $"The property expression [{property.GetType()}] is related to a source parameter / entity " +
                $"expression of type [{parameterExpression.Type}] which is not the same as the TEntity generic parameter " +
                $"[ToCountExpression<{typeof(TEntity)}>].",
                nameof(property));
        }
        
        var underlyingType = property.Type.GetUnderlyingTypeIfNullable();
        var isGenericEnumeration = underlyingType.IsGenericEnumeration();
        var isArray = underlyingType.IsArray;
        var collectionItemType = isGenericEnumeration
            ? underlyingType.GetGenericArguments()[0]
            : (isArray ? underlyingType.GetElementType()! : underlyingType);
        
        var enumerableCountMethod = typeof(Enumerable).GetMethods()
            .First(method => method.Name == "Count" && method.GetParameters().Length == 1)
            .MakeGenericMethod(collectionItemType);
        var expression = Expression.Call(enumerableCountMethod, property);
        return Expression.Lambda<Func<TEntity, int>>(expression, parameterExpression);
    }
}
