using System.Reflection;
using Saorsa.QueryEngine.Annotations;
using Saorsa.QueryEngine.Model;

namespace Saorsa.QueryEngine;

public static class QueryEngine
{
    public static readonly Dictionary<Type, string> SimpleTypeStringMap = new()
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

    public static readonly PropertyFilterDefinition[] FilterTypesNumerals =
    {
        new ("IS_NULL"),
        new ("IS_NOT_NULL"),
        new ("EQ", "<equal-to>"),
        new ("NOT_EQ", "<not-equal-to>"),
        new ("LT", "<less-than>"),
        new ("LT_EQ", "<less-than-or-equal>"),
        new ("GT", "<greater-than>"),
        new ("GE_EQ", "<greater-than-or-equal>"),
        new ("RANGE", "<greater-than-or-equal>", "<less-than-or-equal>"),
        new ("SEQ", "<comma-separated-allowed-values>"),
    };
    
    public static readonly PropertyFilterDefinition[] FilterTypesText =
    {
        new ("IS_NULL"),
        new ("IS_NOT_NULL"),
        new ("EQ", "<equal-to>"),
        new ("NOT_EQ", "<not-equal-to>"),
        new ("CONTAINS", "<contained-text-value>"),
    };

    public static readonly PropertyFilterDefinition[] FilterTypesEnum =
    {
        new ("IS_NULL"),
        new ("IS_NOT_NULL"),
        new ("EQ", "<equal-to>"),
        new ("NOT_EQ", "<not-equal-to>"),
        new ("SEQ", "<comma-separated-allowed-values>"),
    };
    
    public static readonly PropertyFilterDefinition[] FilterTypesArray =
    {
        new ("IS_EMPTY"),
        new ("IS_NOT_EMPTY"),
        new ("NAX_COUNT", "<less-than-or-equal>"),
        new ("MIN_COUNT", "<greater-than-or-equal>"),
    };

    public static readonly Dictionary<Type, object> SimpleTypeFilterTypeMap = new()
    {
        { typeof(char), FilterTypesNumerals },
        { typeof(string), FilterTypesText },
        { typeof(bool), FilterTypesNumerals },
        { typeof(byte), FilterTypesNumerals },
        { typeof(sbyte), FilterTypesNumerals },
        { typeof(short), FilterTypesNumerals },
        { typeof(int), FilterTypesNumerals },
        { typeof(long), FilterTypesNumerals }, 
        { typeof(ushort), FilterTypesNumerals },
        { typeof(uint), FilterTypesNumerals },
        { typeof(ulong), FilterTypesNumerals },
        
        { typeof(decimal), FilterTypesNumerals },
        { typeof(Guid), FilterTypesNumerals },
        { typeof(DateOnly), FilterTypesNumerals },
        { typeof(DateTime), FilterTypesNumerals },
        { typeof(DateTimeOffset), FilterTypesNumerals },
        { typeof(TimeSpan), FilterTypesNumerals },
        { typeof(TimeOnly), FilterTypesNumerals },
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
            .Select(BuildTypePropertyDefinition);

        return properties.ToArray();
    }

    public static PropertyDefinition BuildTypePropertyDefinition(PropertyInfo propertyInfo)
    {
        var underlyingType = propertyInfo.PropertyType.GetUnderlyingTypeIfNullable();
        var result = new PropertyDefinition
        {
            Name = propertyInfo.Name.ToCamelCase(),
            Nullable = propertyInfo.PropertyType.IsNullable(),
            Type = SimpleTypes.Contains(underlyingType) 
                ? SimpleTypeStringMap[underlyingType]
                : null
        };

        if (result.Type != null) return result;
        
        if (underlyingType.IsEnum)
        {
            result.Type = $"[{string.Join("|", Enum.GetNames(underlyingType))}]";
        }
        else
        {
            result.Type = underlyingType.IsSingleElementTypeEnumeration() ? "array" : "reference";    
        }
        return result;
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

