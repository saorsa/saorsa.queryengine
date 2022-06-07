using System.Linq.Expressions;

namespace Saorsa.QueryEngine;

public static class QueryEngineUtil
{
    public static bool IsEnumerable(Type type)
    {
        return type.IsEnumerable();
    }
    
    public static bool IsEnumerable(string typeName)
    {
        var type = Type.GetType(typeName);
        if (type == null)
        {
            throw new ArgumentException(
                $"Unknown type '{typeName}'.");
        }

        return type.IsEnumerable();
    }
    
    public static Expression<Func<TEntity, object>> ToLambda<TEntity>(
        string propertyName)
    {
        var parameter = Expression.Parameter(typeof(TEntity));
        var property = Expression.Property(parameter, propertyName);
        var propAsObject = Expression.Convert(property, typeof(object));

        return Expression.Lambda<Func<TEntity, object>>(propAsObject, parameter);
    }
}
