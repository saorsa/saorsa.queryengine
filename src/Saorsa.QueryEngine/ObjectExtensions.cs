using System.Text.Json;

namespace Saorsa.QueryEngine;

public static class ObjectExtensions
{
    public static TAtom? ConvertToAtom<TAtom>(this object? source)
    {
        return QueryEngine.ConvertToAtom<TAtom>(source);
    }
    
    public static object? ConvertToAtom(
        this object? source,
        Type targetType)
    {
        return QueryEngine.ConvertToAtom(source, targetType);
    }
    
    
    public static object? ConvertToAtom(
        this JsonElement source,
        Type targetType)
    {
        return QueryEngine.ConvertToAtom(source, targetType);
    }
    
    public static TAtom? ConvertToAtom<TAtom>(this JsonElement source)
    {
        return QueryEngine.ConvertToAtom<TAtom>(source);
    }
}
