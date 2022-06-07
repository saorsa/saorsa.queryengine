using System.Collections;

namespace Saorsa.QueryEngine;

public static class TypeExtensions
{
    public static bool IsEnumerable(this Type type)
    {
        return /*type.IsGenericType && */ typeof(IEnumerable).IsAssignableFrom(type);
    }
}
