using System.Reflection;
using Saorsa.QueryEngine.Model;

namespace Saorsa.QueryEngine;

public static partial class QueryEngine
{
    private static readonly object CompileLock = new();
    private static readonly Dictionary<int, Dictionary<Type, TypeDefinition>> CompileMap = new();

    public static TypeDefinition[] CompileTypeDefinitions(
        int maxDepth = DefaultTypeDefinitionDepth,
        bool overrideIgnores = false)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => CompileTypeDefinitions(a, maxDepth, overrideIgnores))
            .ToArray();
    }
    
    public static TypeDefinition[] CompileTypeDefinitions(
        Assembly assembly,
        int maxDepth = DefaultTypeDefinitionDepth,
        bool overrideIgnores = false)
    {
        var compiled = new List<TypeDefinition>();
        ScanQueryEngineTypes(assembly)
            .ToList()
            .ForEach(t =>
            {
                var typeDef = BuildTypeDefinition(t, maxDepth, overrideIgnores);
                if (typeDef == null) return;
                lock (CompileLock)
                {
                    if (!CompileMap.ContainsKey(maxDepth))
                    {
                        CompileMap.Add(maxDepth, new Dictionary<Type, TypeDefinition>());
                    }

                    if (CompileMap[maxDepth].ContainsKey(t)) return;
                    
                    CompileMap[maxDepth][t] = typeDef;
                    compiled.Add(typeDef);
                }
            });
        return compiled.ToArray();
    }
}
