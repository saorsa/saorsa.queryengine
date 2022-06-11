using System.Reflection;

namespace Saorsa.QueryEngine;

public static partial class QueryEngine
{
    public static Type[] ScanQueryEngineTypes()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(ScanQueryEngineTypes)
            .ToArray();
    }
    
    public static Type[] ScanQueryEngineTypes(Assembly assembly)
    {
        return assembly
            .GetTypes()
            .Where(t => t.IsQueryEngineCompiled())
            .ToArray();
    }
}