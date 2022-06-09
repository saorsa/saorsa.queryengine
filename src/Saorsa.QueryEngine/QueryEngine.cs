using System.Reflection;
using Saorsa.QueryEngine.Annotations;
using Saorsa.QueryEngine.Model;

namespace Saorsa.QueryEngine;

public static class QueryEngine
{
    public static readonly Dictionary<Type, string> SimpleTypeStringMap = new Dictionary<Type, string>
    {
        { typeof(char), "char" },
        { typeof(string), "string" },
        { typeof(bool), "boolean" },
        { typeof(byte), "byte" },
        { typeof(sbyte), "sbyte" },
        { typeof(short), "int16" },
        { typeof(int), "integer" },
        { typeof(long), "int64" }, 
        { typeof(ushort), "uint16" },
        { typeof(uint), "uint32" },
        { typeof(ulong), "uint64" },
        
        { typeof(decimal), "decimal" },
        { typeof(Guid), "uuid" },
        { typeof(DateOnly), "date" },
        { typeof(DateTime), "dateTime" },
        { typeof(DateTimeOffset), "dateTimeOffset" },
        { typeof(TimeSpan), "timeSpan" },
        { typeof(TimeOnly), "time" },
    };

    public static readonly IEnumerable<Type> SimpleTypes = SimpleTypeStringMap.Keys;

    public static PropertyDefinition[] BuildTypePropertyDefinitions<TEntity>()
    {
        return BuildTypePropertyDefinitions(typeof(TEntity));
    }

    public static PropertyDefinition[] BuildTypePropertyDefinitions(Type type)
    {
        if (!type.IsClass)
        {
            return Array.Empty<PropertyDefinition>();
        }

        if (SimpleTypes.Contains(type))
        {
            return Array.Empty<PropertyDefinition>();
        }

        if (type.IsArray || type.IsGenericEnumeration())
        {
            return Array.Empty<PropertyDefinition>();
        }
        
        if (IsIgnoredByQueryEngine(type))
        {
            return Array.Empty<PropertyDefinition>();
        }

        var properties = type
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
            .Where(p => !IsIgnoredByQueryEngine(p))
            .Select(p => new PropertyDefinition
            {
                Name = p.Name.ToCamelCase(),
                Type = SimpleTypes.Contains(p.PropertyType) 
                    ? SimpleTypeStringMap[p.PropertyType]
                    : p.PropertyType.IsEnum ? $"[{string.Join("|", Enum.GetNames(p.PropertyType))}]" : "composite"
            });

        return properties.ToArray();
    }

    public static bool IsIgnoredByQueryEngine(PropertyInfo propertyInfo)
    {
        return propertyInfo
            .GetCustomAttributes(typeof(QueryEngineIgnoreAttribute), true)
            .Any();
    }
    
    public static bool IsIgnoredByQueryEngine(Type type)
    {
        return type
            .GetCustomAttributes(typeof(QueryEngineIgnoreAttribute), true)
            .Any();
    }
}

