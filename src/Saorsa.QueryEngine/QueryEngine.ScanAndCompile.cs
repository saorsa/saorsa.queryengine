using System.Reflection;
using Saorsa.QueryEngine.Model;

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
            .ForEach(type =>
            {
                var typeDef = CompileType(type, maxDepth, overrideIgnores);
                if (typeDef != null)
                {
                    compiled.Add(typeDef);
                }
            });
        return compiled.ToArray();
    }

    public static TypeDefinition? CompileType<TEntity>(
        int maxDepth = DefaultTypeDefinitionDepth,
        bool overrideIgnores = false)
    {
        return CompileType(typeof(TEntity), maxDepth, overrideIgnores);
    }
    
    public static TypeDefinition? CompileType(
        Type type,
        int maxDepth = DefaultTypeDefinitionDepth,
        bool overrideIgnores = false)
    {
        var typeDef = BuildTypeDefinition(type, maxDepth, overrideIgnores);
        if (typeDef == null) return null;
        lock (CompileLock)
        {
            if (!CompileMap.ContainsKey(maxDepth))
            {
                CompileMap.Add(maxDepth, new Dictionary<Type, TypeDefinition>());
            }
            CompileMap[maxDepth][type] = typeDef;
            return typeDef;
        }
    }

    public static TypeDefinition? GetCompiled<TEntity>(
        int maxDepth = DefaultTypeDefinitionDepth)
    {
        return GetCompiled(typeof(TEntity), maxDepth);
    }
    
    public static TypeDefinition? GetCompiled(
        Type type,
        int maxDepth = DefaultTypeDefinitionDepth)
    {
        lock (CompileLock)
        {
            if (!CompileMap.ContainsKey(maxDepth))
            {
                return null;
            }
            
            return !CompileMap[maxDepth].ContainsKey(type)
                ? null
                : CompileMap[maxDepth][type];
        }
    }
    
    public static bool IsCompiled<TEntity>(
        int maxDepth = DefaultTypeDefinitionDepth)
    {
        return IsCompiled(typeof(TEntity), maxDepth);
    }
    
    public static bool IsCompiled(
        Type type,
        int maxDepth = DefaultTypeDefinitionDepth)
    {
        return GetCompiled(type, maxDepth) != null;
    }
}