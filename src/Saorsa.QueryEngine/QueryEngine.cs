using System.Linq.Expressions;
using System.Reflection;
using Saorsa.QueryEngine.Annotations;
using Saorsa.QueryEngine.Model;

namespace Saorsa.QueryEngine;

public static partial class QueryEngine
{
    // https://blog.jeremylikness.com/blog/dynamically-build-linq-expressions/

    public const int DefaultTypeDefinitionDepth = 3;

    public static QueryableTypeDescriptor? BuildTypeDefinition<TEntity>(
        int maxReferenceDepth = DefaultTypeDefinitionDepth,
        string? name = null,
        bool overrideIgnores = false)
    {
        return BuildTypeDefinition(typeof(TEntity), maxReferenceDepth, name, overrideIgnores);
    }

    public static QueryableTypeDescriptor? BuildTypeDefinition(
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
        
        var result = new QueryableTypeDescriptor
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

    public static QueryableTypeDescriptor GetPropertyDefinitionOrThrow<TEntity>(
        QueryableTypeDescriptor queryableTypeDef,
        PropertyFilter propertyFilter)
    {
        var matchingTypeDef = queryableTypeDef.Properties?.FirstOrDefault(p =>
            p.Name.Equals(propertyFilter.Name.ToCamelCase()));
        if (matchingTypeDef == null)
        {
            throw new QueryEngineException(
                ErrorCodes.PropertyNotFoundError, 
                $"Type {typeof(TEntity)} does not have a property named {propertyFilter.Name}.");
        }

        var filterDef = matchingTypeDef.AllowedFilters.FirstOrDefault(f => f.OperatorType == propertyFilter.FilterType);
        if (filterDef == null)
        {
            throw new QueryEngineException(
                ErrorCodes.PropertyFilterInvalidError, 
                $"Type {typeof(TEntity)}, property {propertyFilter.Name} does not support filter {propertyFilter.FilterType}.");
        }

        if (filterDef.Arg1Required.GetValueOrDefault())
        {
            if (propertyFilter.Arguments.Length < 1)
            { 
                throw new QueryEngineException(
                ErrorCodes.ArgumentLengthError,
                $"Type {typeof(TEntity)}, property {propertyFilter.Name}, filter " +
                $"{propertyFilter.FilterType} expects a single argument which is not provided.");
            }
        }
        else if (propertyFilter.Arguments.Length > 0)
        {
            throw new QueryEngineException(
                ErrorCodes.ArgumentLengthError,
                $"Type {typeof(TEntity)}, property {propertyFilter.Name}, filter " +
                $"{propertyFilter.FilterType} does not expect an argument, but it was provided.");
        }
        
        if (filterDef.Arg2Required.GetValueOrDefault())
        {
            if (propertyFilter.Arguments.Length < 2)
            { 
                throw new QueryEngineException(
                    ErrorCodes.ArgumentLengthError,
                    $"Type {typeof(TEntity)}, property {propertyFilter.Name}, filter " +
                    $"{propertyFilter.FilterType} expects a second argument which was not provided.");
            }
        }
        else if (propertyFilter.Arguments.Length > 1)
        {
            throw new QueryEngineException(
                ErrorCodes.ArgumentLengthError,
                $"Type {typeof(TEntity)}, property {propertyFilter.Name}, filter " +
                $"{propertyFilter.FilterType} does not expect a second argument, but it was provided.");
        }

        return matchingTypeDef;
    }

    public static IQueryable<TEntity> AddPropertyFilter<TEntity>(
        IQueryable<TEntity> query,
        PropertyFilter propertyFilter)
    {
        var expression = ToExpression<TEntity>(propertyFilter);
        return query.Where(expression);
    }

    public static IQueryable<TEntity> AddPropertyFilterBlock<TEntity>(
        IQueryable<TEntity> query,
        PropertyFilterBlock block)
    {
        var expression = ToExpression<TEntity>(block);
        return query.Where(expression);
    }

    public static Expression<Func<TEntity, bool>> ToExpression<TEntity>(
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
            case FilterOperatorType.IsNull:
            {
                return ExpressionBuilder.PropertyIsNull<TEntity>(
                    propertyDef.Name);
            }

            case FilterOperatorType.IsNotNull:
            {
                return ExpressionBuilder.PropertyIsNotNull<TEntity>(
                    propertyDef.Name);
            }

            case FilterOperatorType.EqualTo:
            {
                return ExpressionBuilder.PropertyEqualTo<TEntity>(
                    propertyDef.Name,
                    propertyFilter.Arguments[0]);
            }

            case FilterOperatorType.NotEqualTo:
            {
                return ExpressionBuilder.PropertyNotEqualTo<TEntity>(
                    propertyDef.Name,
                    propertyFilter.Arguments[0]);
            }

            case FilterOperatorType.LessThan:
            {
                return ExpressionBuilder.PropertyLessThan<TEntity>(
                    propertyDef.Name,
                    propertyFilter.Arguments[0]);
            }

            case FilterOperatorType.LessThanOrEqual:
            {
                return ExpressionBuilder.PropertyLessThanOrEqual<TEntity>(
                    propertyDef.Name,
                    propertyFilter.Arguments[0]);
            }

            case FilterOperatorType.GreaterThan:
            {
                return ExpressionBuilder.PropertyGreaterThan<TEntity>(
                    propertyDef.Name,
                    propertyFilter.Arguments[0]);
            }

            case FilterOperatorType.GreaterThanOrEqual:
            {
                return ExpressionBuilder.PropertyGreaterThanOrEqual<TEntity>(
                    propertyDef.Name,
                    propertyFilter.Arguments[0]);
            }

            default:
                throw new QueryEngineException(1400, 
                    $"Type {typeof(TEntity)}, property {propertyFilter.Name}, filter {propertyFilter.FilterType}" +
                    $"is not implemented yet.");
        }
    }

    public static Expression<Func<TEntity, bool>> ToExpression<TEntity>(
        PropertyFilterBlock block,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TEntity));
        var expression = ToBinaryExpression<TEntity>(block, parameter);
        return Expression.Lambda<Func<TEntity, bool>>(expression, parameter);
    }

    public static BinaryExpression ToBinaryExpression<TEntity>(
        PropertyFilter propertyFilter,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TEntity));
        var typeDef = EnsureCompiled<TEntity>();
        if (typeDef == null)
        {
            throw new QueryEngineException(
                ErrorCodes.TypeDefinitionCompilationError, 
                $"Type {typeof(TEntity)} is not supported by query engine.");
        }
        
        var propertyDef = GetPropertyDefinitionOrThrow<TEntity>(typeDef, propertyFilter);

        return propertyFilter.FilterType switch
        {
            FilterOperatorType.IsNull => ExpressionBuilder.PropertyIsNullExpression<TEntity>(propertyDef.Name, parameter),
            FilterOperatorType.IsNotNull => ExpressionBuilder.PropertyIsNotNullExpression<TEntity>(propertyDef.Name,
                parameter),
            FilterOperatorType.EqualTo => ExpressionBuilder.PropertyEqualToExpression<TEntity>(propertyDef.Name,
                propertyFilter.Arguments[0], parameter),
            FilterOperatorType.NotEqualTo => ExpressionBuilder.PropertyNotEqualToExpression<TEntity>(propertyDef.Name,
                propertyFilter.Arguments[0], parameter),
            FilterOperatorType.LessThan => ExpressionBuilder.PropertyLessThanExpression<TEntity>(propertyDef.Name,
                propertyFilter.Arguments[0], parameter),
            FilterOperatorType.LessThanOrEqual => ExpressionBuilder.PropertyLessThanOrEqualExpression<TEntity>(propertyDef.Name,
                propertyFilter.Arguments[0], parameter),
            FilterOperatorType.GreaterThan => ExpressionBuilder.PropertyGreaterThanExpression<TEntity>(propertyDef.Name,
                propertyFilter.Arguments[0], parameter),
            FilterOperatorType.GreaterThanOrEqual => ExpressionBuilder.PropertyGreaterThanOrEqualExpression<TEntity>(propertyDef.Name,
                propertyFilter.Arguments[0], parameter),
            FilterOperatorType.StringContains => ExpressionBuilder.BuildContainsExpression<TEntity>(
                propertyDef.Name,
                propertyFilter.Arguments[0],
                parameter),
            _ => throw new QueryEngineException(ErrorCodes.PropertyFilterNotImplementedError,
                $"Type {typeof(TEntity)}, property {propertyFilter.Name}, filter {propertyFilter.FilterType}" +
                $"is not implemented yet.")
        };
    }
    
    public static BinaryExpression ToBinaryExpression<TEntity>(
        PropertyFilterBlock block,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TEntity));
        var result = ToBinaryExpression<TEntity>(block.First, parameter);
        
        if (block.IsComposite)
        {
            var condition = block.Condition ?? LogicalOperator.And;
                
            block.Others.ToList().ForEach(item =>
            {
                var other = ToBinaryExpression<TEntity>(item, parameter);
                var combined = condition == LogicalOperator.And
                    ? Expression.And(result, other)
                    : Expression.Or(result, other);
                result = combined;
            });

            if (result.CanReduce)
            {
                result.ReduceAndCheck();
            }

            return result;
        }

        return result;
    }
}
