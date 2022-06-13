using System.Linq.Expressions;
using System.Reflection;
using Saorsa.QueryEngine.Annotations;
using Saorsa.QueryEngine.Model;

namespace Saorsa.QueryEngine;

public static partial class QueryEngine
{
    public const int DefaultTypeDefinitionDepth = 2;
    
    public static TypeDefinition? BuildTypeDefinition<TEntity>(
        int maxReferenceDepth = DefaultTypeDefinitionDepth,
        string? name = null,
        bool overrideIgnores = false)
    {
        return BuildTypeDefinition(typeof(TEntity), maxReferenceDepth, name, overrideIgnores);
    }

    public static TypeDefinition? BuildTypeDefinition(
        Type type,
        int maxReferenceDepth = DefaultTypeDefinitionDepth,
        string? name = null,
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
            Name = name ?? underlyingType.Name,
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
                name,
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
                    p.Name.ToCamelCase(),
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

    public static TypeDefinition GetPropertyDefinitionOrThrow<TEntity>(
        TypeDefinition typeDef,
        PropertyFilter propertyFilter)
    {

        var matchingTypeDef = typeDef.Properties?.FirstOrDefault(p =>
            p.Name.Equals(propertyFilter.Name));
        if (matchingTypeDef == null)
        {
            throw new QueryEngineException(1200, 
                $"Type {typeof(TEntity)} does not have a property named {propertyFilter.Name}.");
        }

        var filterDef = matchingTypeDef.AllowedFilters.FirstOrDefault(f => f.FilterType == propertyFilter.FilterType);
        if (filterDef == null)
        {
            throw new QueryEngineException(1300, 
                $"Type {typeof(TEntity)}, property {propertyFilter.Name} does not support filter {propertyFilter.FilterType}.");
        }

        if (filterDef.Arg1Required.GetValueOrDefault())
        {
            if (propertyFilter.Arguments.Length < 1)
            { 
                throw new QueryEngineException(
                2501,
                $"Type {typeof(TEntity)}, property {propertyFilter.Name}, filter " +
                $"{propertyFilter.FilterType} expects a single argument which is not provided.");
            }
        }
        else if (propertyFilter.Arguments.Length > 0)
        {
            throw new QueryEngineException(
                2501,
                $"Type {typeof(TEntity)}, property {propertyFilter.Name}, filter " +
                $"{propertyFilter.FilterType} does not expect an argument, but it was provided.");
        }
        
        if (filterDef.Arg2Required.GetValueOrDefault())
        {
            if (propertyFilter.Arguments.Length < 2)
            { 
                throw new QueryEngineException(
                    2501,
                    $"Type {typeof(TEntity)}, property {propertyFilter.Name}, filter " +
                    $"{propertyFilter.FilterType} expects a second argument which was not provided.");
            }
        }
        else if (propertyFilter.Arguments.Length > 1)
        {
            throw new QueryEngineException(
                2501,
                $"Type {typeof(TEntity)}, property {propertyFilter.Name}, filter " +
                $"{propertyFilter.FilterType} does not expect a second argument, but it was provided.");
        }

        return matchingTypeDef;
    }
    
    public static IQueryable<TEntity> ApplyPropertyFilter<TEntity>(
        IQueryable<TEntity> query,
        PropertyFilter propertyFilter)
    {
        var typeDef = EnsureCompiled<TEntity>();
        
        if (typeDef == null)
        {
            throw new QueryEngineException(1100, 
                $"Type {typeof(TEntity)} is not supported by query engine.");
        }
        
        var propertyDef = GetPropertyDefinitionOrThrow<TEntity>(typeDef, propertyFilter);
        
        switch (propertyFilter.FilterType)
        {
            case FilterType.EQ:
            {
                var expression = ExpressionBuilder.PropertyEqualTo<TEntity>(
                    propertyDef.Name,
                    propertyFilter.Arguments[0]);
                return query.Where(expression);
            }
            
            case FilterType.NOT_EQ:
            {
                var expression = ExpressionBuilder.PropertyNotEqualTo<TEntity>(
                    propertyDef.Name,
                    propertyFilter.Arguments[0]);
                return query.Where(expression);
            }
            
            default:
                throw new QueryEngineException(1400, 
                    $"Type {typeof(TEntity)}, property {propertyFilter.Name}, filter {propertyFilter.FilterType}" +
                    $"is not implemented yet.");
        }
    }
}
