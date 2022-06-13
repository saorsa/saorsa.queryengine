using System.Reflection;
using Saorsa.QueryEngine.Annotations;
using Saorsa.QueryEngine.Model;

namespace Saorsa.QueryEngine;

public static partial class QueryEngine
{
    public const int DefaultTypeDefinitionDepth = 2;
    
    public static TypeDefinition? BuildTypeDefinition<TEntity>(
        int maxReferenceDepth = DefaultTypeDefinitionDepth,
        bool overrideIgnores = false)
    {
        return BuildTypeDefinition(typeof(TEntity), maxReferenceDepth, overrideIgnores);
    }

    public static TypeDefinition? BuildTypeDefinition(
        Type type,
        int maxReferenceDepth = DefaultTypeDefinitionDepth,
        bool overrideIgnores = false)
    {
        if (maxReferenceDepth <= 0)
        {
            return null;
        }

        if (!overrideIgnores && IsIgnoredByQueryEngine(type))
        {
            return null;
        }
        
        var underlyingType = type.GetUnderlyingTypeIfNullable();
        var isSimpleType = underlyingType.IsQueryEngineSimpleType();
        var isCompositeType = !isSimpleType;
        
        var result = new TypeDefinition
        {
            TypeName = underlyingType.Name,
            Nullable = underlyingType.IsNullable(),
            Type = underlyingType.GetQueryEngineStringRepresentation(),
            AllowedFilters = underlyingType.GetQueryEngineFilterDefinitions(),
        };

        if (underlyingType.IsEnum)
        {
            result.EnumValues = Enum.GetNames(underlyingType);
        }
        else if (underlyingType.IsSingleElementTypeEnumeration())
        {
            result.ArrayElement = BuildTypeDefinition(
                underlyingType.GetSingleElementEnumerationType()!,
                maxReferenceDepth,
                overrideIgnores);
        }
        else if (!isSimpleType)
        {
            isCompositeType = true;
        }

        if (isCompositeType && maxReferenceDepth > 1)
        {
            var properties = type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
                .Where(p => overrideIgnores || !IsIgnoredByQueryEngine(p))
                .Select(p => BuildTypeDefinition(
                    p.PropertyType, 
                    maxReferenceDepth - 1,
                    overrideIgnores))
                .Where(t => t != null)
                .ToArray();

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
    
    public static bool IsCompiledByQueryEngine(Type type)
    {
        return type
            .GetCustomAttributes(typeof(QueryEngineCompileAttribute), true)
            .Any();
    }

    public static IQueryable<TEntity> Query<TEntity>(
        IQueryable<TEntity> source)
    {
        return source;
    }
}
