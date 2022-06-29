using Saorsa.QueryEngine.Model;

namespace Saorsa.QueryEngine;

public static class TypeExtensions
{
    public static bool IsQueryEngineSimpleType(this Type type)
    {
        return QueryEngine.IsAtomicType(type);
    }
    
    public static bool IsQueryEngineIgnored(this Type type)
    {
        return QueryEngine.IsIgnoredByQueryEngine(type);
    }
    
    public static bool IsQueryEngineCompiled(this Type type)
    {
        return QueryEngine.IsCompiledByQueryEngine(type);
    }
    
    public static string GetQueryEngineStringRepresentation(this Type type)
    {
        return QueryEngine.GetStringRepresentation(type);
    }

    public static FilterDefinition[] GetQueryEngineFilterDefinitions(this Type type)
    {
        return QueryEngine.GetFilterDefinitions(type);
    }

    public static object? GetDefaultValue(this Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}
