using Saorsa.QueryEngine.Model;

namespace Saorsa.QueryEngine;

public static class TypeExtensions
{
    public static bool IsQueryEngineSimpleType(this Type type)
    {
        return QueryEngine.IsSimpleType(type);
    }
    
    public static string GetQueryEngineStringRepresentation(this Type type)
    {
        return QueryEngine.GetStringRepresentation(type);
    }

    public static FilterDefinition[] GetQueryEngineFilterDefinitions(this Type type)
    {
        return QueryEngine.GetFilterDefinitions(type);
    }
}
