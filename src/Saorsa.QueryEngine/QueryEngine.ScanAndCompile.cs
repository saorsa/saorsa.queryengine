using System.Reflection;
using Saorsa.QueryEngine.Model;

namespace Saorsa.QueryEngine;

public static partial class QueryEngine
{
    private static readonly object CompileLock = new();
    private static readonly object EnsureCompileLock = new();
    private static readonly Dictionary<int, Dictionary<Type, QueryableTypeDescriptor>> CompileMap = new();
    
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

    public static QueryableTypeDescriptor[] CompileTypeDefinitions(
        int maxDepth = DefaultTypeDefinitionDepth,
        bool overrideIgnores = false)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => CompileTypeDefinitions(a, maxDepth, overrideIgnores))
            .ToArray();
    }
    
    public static QueryableTypeDescriptor[] CompileTypeDefinitions(
        Assembly assembly,
        int maxDepth = DefaultTypeDefinitionDepth,
        bool overrideIgnores = false)
    {
        var compiled = new List<QueryableTypeDescriptor>();
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

    public static QueryableTypeDescriptor? CompileType<TEntity>(
        int maxDepth = DefaultTypeDefinitionDepth,
        bool overrideIgnores = false)
    {
        return CompileType(typeof(TEntity), maxDepth, overrideIgnores);
    }
    
    public static QueryableTypeDescriptor? CompileType(
        Type type,
        int maxDepth = DefaultTypeDefinitionDepth,
        bool overrideIgnores = false)
    {
        var typeDef = BuildTypeDefinition(type, maxDepth, null, overrideIgnores);
        if (typeDef == null) return null;
        lock (CompileLock)
        {
            if (!CompileMap.ContainsKey(maxDepth))
            {
                CompileMap.Add(maxDepth, new Dictionary<Type, QueryableTypeDescriptor>());
            }
            CompileMap[maxDepth][type] = typeDef;
            return typeDef;
        }
    }

    public static QueryableTypeDescriptor? GetCompiled<TEntity>(
        int maxDepth = DefaultTypeDefinitionDepth)
    {
        return GetCompiled(typeof(TEntity), maxDepth);
    }
    
    public static QueryableTypeDescriptor? GetCompiled(
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

    public static QueryableTypeDescriptor? EnsureCompiled<TEntity>(
        int maxDepth = DefaultTypeDefinitionDepth)
    {
        return EnsureCompiled(typeof(TEntity), maxDepth);
    }
    
    public static QueryableTypeDescriptor? EnsureCompiled(
        Type type,
        int maxDepth = DefaultTypeDefinitionDepth)
    {
        lock (EnsureCompileLock)
        {
            var compiled = GetCompiled(type, maxDepth) ?? CompileType(type, maxDepth);
            return compiled;
        }
    }
}
