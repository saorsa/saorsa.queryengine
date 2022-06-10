using System.Reflection;
using Saorsa.QueryEngine.Annotations;
using Saorsa.QueryEngine.Model;

namespace Saorsa.QueryEngine;

public static class QueryEngine
{
    
    public static readonly Dictionary<Type, FilterDefinition[]> SimpleTypeFilterTypeMap = new()
    {
        { typeof(char), FilterDefinition.NumericFilters },
        { typeof(string), FilterDefinition.TextFilters  },
        { typeof(bool), FilterDefinition.NumericFilters  },
        { typeof(byte), FilterDefinition.NumericFilters  },
        { typeof(sbyte), FilterDefinition.NumericFilters  },
        { typeof(short), FilterDefinition.NumericFilters  },
        { typeof(int), FilterDefinition.NumericFilters  },
        { typeof(long), FilterDefinition.NumericFilters  }, 
        { typeof(ushort), FilterDefinition.NumericFilters  },
        { typeof(uint), FilterDefinition.NumericFilters  },
        { typeof(ulong), FilterDefinition.NumericFilters  },
        
        { typeof(decimal), FilterDefinition.NumericFilters  },
        { typeof(Guid), FilterDefinition.NumericFilters  },
        { typeof(DateOnly), FilterDefinition.NumericFilters  },
        { typeof(DateTime), FilterDefinition.NumericFilters  },
        { typeof(DateTimeOffset), FilterDefinition.NumericFilters  },
        { typeof(TimeSpan), FilterDefinition.NumericFilters  },
        { typeof(TimeOnly), FilterDefinition.NumericFilters  },
    };
    
    public static readonly IEnumerable<Type> SimpleTypes = SimpleTypeStringMap.Keys;

    public static TypeDefinition? BuildTypeDefinition<TEntity>(int maxReferenceDepth = 2)
    {
        return BuildTypeDefinition(typeof(TEntity), maxReferenceDepth);
    }

    public static TypeDefinition? BuildTypeDefinition(Type type, int maxReferenceDepth = 2)
    {
        if (maxReferenceDepth <= 0)
        {
            return null;
        }

        if (IsIgnoredByQueryEngine(type))
        {
            return null;
        }
        
        var underlyingType = type.GetUnderlyingTypeIfNullable();
        var isSimpleType = SimpleTypes.Contains(underlyingType);
        var isCompositeType = false;
        
        var result = new TypeDefinition
        {
            TypeName = underlyingType.Name.ToPascalCase(),
            Nullable = underlyingType.IsNullable(),
            Type = isSimpleType ? SimpleTypeStringMap[underlyingType] : null,
            AllowedFilters = isSimpleType ? SimpleTypeFilterTypeMap[underlyingType] : Array.Empty<FilterDefinition>()
        };

        if (underlyingType.IsEnum)
        {
            result.Type = "enum";
            result.EnumValues = Enum.GetNames(underlyingType);
            result.AllowedFilters = FilterDefinition.EnumFilters;
        }
        else if (underlyingType.IsSingleElementTypeEnumeration())
        {
            result.Type = "array";
            result.AllowedFilters = FilterDefinition.ArrayFilters;
            result.ReferenceType = BuildTypeDefinition(underlyingType, maxReferenceDepth - 1);
        }
        else if (result.Type != null)
        {
            isCompositeType = true;
            result.Type = "reference";
            result.AllowedFilters = FilterDefinition.ReferenceFilters;
            result.ReferenceType = BuildTypeDefinition(underlyingType, maxReferenceDepth - 1);
        }

        if (isCompositeType && maxReferenceDepth > 1)
        {
            var properties = type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
                .Where(p => !IsIgnoredByQueryEngine(p))
                .Select(p => BuildTypeDefinition(p.PropertyType, maxReferenceDepth - 1))
                .Where(t => t != null);

            result.Properties = properties.Any() 
                ? properties.Select(t => t!).ToArray()
                : null;
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

