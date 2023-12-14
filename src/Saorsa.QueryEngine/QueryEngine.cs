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
        FilterPropertyDescriptor filterPropertyDescriptor)
    {
        var sequence = filterPropertyDescriptor.AsSequence();
        QueryableTypeDescriptor sequenceDescriptor = queryableTypeDef;

        if (sequence.Length == 0)
        {
            throw new ArgumentException(
                $"Property filter {filterPropertyDescriptor.Name} is invalid.");
        }
        
        foreach (var property in sequence)
        {
            var nextDescriptor = sequenceDescriptor
                .Properties?
                .FirstOrDefault(p => p.Name.Equals(property.ToCamelCase()));
            if (nextDescriptor == null)
            {
                throw new QueryEngineException(
                    ErrorCodes.PropertyNotFoundError, 
                    $"Type {typeof(TEntity)} does not have a property named {filterPropertyDescriptor.Name}.");
            }

            sequenceDescriptor = nextDescriptor;
        }

        var filterDef = sequenceDescriptor.AllowedFilters.FirstOrDefault(f => f.OperatorType == filterPropertyDescriptor.FilterType);
        if (filterDef == null)
        {
            throw new QueryEngineException(
                ErrorCodes.PropertyFilterInvalidError, 
                $"Type {typeof(TEntity)}, property {filterPropertyDescriptor.Name} does not support filter {filterPropertyDescriptor.FilterType}.");
        }

        if (filterDef.Arg1Required.GetValueOrDefault())
        {
            if (filterPropertyDescriptor.Arguments.Length < 1)
            { 
                throw new QueryEngineException(
                ErrorCodes.ArgumentLengthError,
                $"Type {typeof(TEntity)}, property {filterPropertyDescriptor.Name}, filter " +
                $"{filterPropertyDescriptor.FilterType} expects a single argument which is not provided.");
            }
        }
        else if (filterPropertyDescriptor.Arguments.Length > 0)
        {
            throw new QueryEngineException(
                ErrorCodes.ArgumentLengthError,
                $"Type {typeof(TEntity)}, property {filterPropertyDescriptor.Name}, filter " +
                $"{filterPropertyDescriptor.FilterType} does not expect an argument, but it was provided.");
        }
        
        if (filterDef.Arg2Required.GetValueOrDefault())
        {
            if (filterPropertyDescriptor.Arguments.Length < 2)
            { 
                throw new QueryEngineException(
                    ErrorCodes.ArgumentLengthError,
                    $"Type {typeof(TEntity)}, property {filterPropertyDescriptor.Name}, filter " +
                    $"{filterPropertyDescriptor.FilterType} expects a second argument which was not provided.");
            }
        }
        else if (filterPropertyDescriptor.Arguments.Length > 1)
        {
            throw new QueryEngineException(
                ErrorCodes.ArgumentLengthError,
                $"Type {typeof(TEntity)}, property {filterPropertyDescriptor.Name}, filter " +
                $"{filterPropertyDescriptor.FilterType} does not expect a second argument, but it was provided.");
        }

        if (sequence.Length > 1)
        {
            sequenceDescriptor.Name = filterPropertyDescriptor.Name;
        }
        return sequenceDescriptor;
    }

    public static IQueryable<TEntity> AddPropertyFilter<TEntity>(
        IQueryable<TEntity> query,
        FilterPropertyDescriptor filterPropertyDescriptor)
    {
        var expression = ToExpression<TEntity>(filterPropertyDescriptor);
        return query.Where(expression);
    }

    public static IQueryable<TEntity> AddPropertyFilterBlock<TEntity>(
        IQueryable<TEntity> query,
        FilterBlockDescriptor blockDescriptor)
    {
        var expression = ToExpression<TEntity>(blockDescriptor);
        return query.Where(expression);
    }

    public static Expression<Func<TEntity, bool>> ToExpression<TEntity>(
        FilterPropertyDescriptor filterPropertyDescriptor)
    {
        var typeDef = EnsureCompiled<TEntity>();

        if (typeDef == null)
        {
            throw new QueryEngineException(1100, 
                $"Type {typeof(TEntity)} is not supported by query engine.");
        }

        var propertyDef = GetPropertyDefinitionOrThrow<TEntity>(typeDef, filterPropertyDescriptor);

        switch (filterPropertyDescriptor.FilterType)
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
                    filterPropertyDescriptor.Arguments[0]);
            }

            case FilterOperatorType.NotEqualTo:
            {
                return ExpressionBuilder.PropertyNotEqualTo<TEntity>(
                    propertyDef.Name,
                    filterPropertyDescriptor.Arguments[0]);
            }

            case FilterOperatorType.LessThan:
            {
                return ExpressionBuilder.PropertyLessThan<TEntity>(
                    propertyDef.Name,
                    filterPropertyDescriptor.Arguments[0]);
            }

            case FilterOperatorType.LessThanOrEqual:
            {
                return ExpressionBuilder.PropertyLessThanOrEqual<TEntity>(
                    propertyDef.Name,
                    filterPropertyDescriptor.Arguments[0]);
            }

            case FilterOperatorType.GreaterThan:
            {
                return ExpressionBuilder.PropertyGreaterThan<TEntity>(
                    propertyDef.Name,
                    filterPropertyDescriptor.Arguments[0]);
            }

            case FilterOperatorType.GreaterThanOrEqual:
            {
                return ExpressionBuilder.PropertyGreaterThanOrEqual<TEntity>(
                    propertyDef.Name,
                    filterPropertyDescriptor.Arguments[0]);
            }

            default:
                throw new QueryEngineException(1400, 
                    $"Type {typeof(TEntity)}, property {filterPropertyDescriptor.Name}, filter {filterPropertyDescriptor.FilterType}" +
                    $"is not implemented yet.");
        }
    }

    public static Expression<Func<TEntity, bool>> ToExpression<TEntity>(
        FilterBlockDescriptor blockDescriptor,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TEntity));
        var expression = ToBinaryExpression<TEntity>(blockDescriptor, parameter);
        return Expression.Lambda<Func<TEntity, bool>>(expression, parameter);
    }

    public static BinaryExpression ToBinaryExpression<TEntity>(
        FilterPropertyDescriptor filterPropertyDescriptor,
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
        
        var propertyDef = GetPropertyDefinitionOrThrow<TEntity>(typeDef, filterPropertyDescriptor);

        return filterPropertyDescriptor.FilterType switch
        {
            FilterOperatorType.IsNull => ExpressionBuilder.PropertyIsNullExpression<TEntity>(propertyDef.Name, parameter),
            FilterOperatorType.IsNotNull => ExpressionBuilder.PropertyIsNotNullExpression<TEntity>(propertyDef.Name,
                parameter),
            FilterOperatorType.EqualTo => ExpressionBuilder.PropertyEqualToExpression<TEntity>(propertyDef.Name,
                filterPropertyDescriptor.Arguments[0], parameter),
            FilterOperatorType.NotEqualTo => ExpressionBuilder.PropertyNotEqualToExpression<TEntity>(propertyDef.Name,
                filterPropertyDescriptor.Arguments[0], parameter),
            FilterOperatorType.LessThan => ExpressionBuilder.PropertyLessThanExpression<TEntity>(propertyDef.Name,
                filterPropertyDescriptor.Arguments[0], parameter),
            FilterOperatorType.LessThanOrEqual => ExpressionBuilder.PropertyLessThanOrEqualExpression<TEntity>(propertyDef.Name,
                filterPropertyDescriptor.Arguments[0], parameter),
            FilterOperatorType.GreaterThan => ExpressionBuilder.PropertyGreaterThanExpression<TEntity>(propertyDef.Name,
                filterPropertyDescriptor.Arguments[0], parameter),
            FilterOperatorType.GreaterThanOrEqual => ExpressionBuilder.PropertyGreaterThanOrEqualExpression<TEntity>(propertyDef.Name,
                filterPropertyDescriptor.Arguments[0], parameter),
            FilterOperatorType.StringContains => ExpressionBuilder.BuildContainsExpression<TEntity>(
                propertyDef.Name,
                filterPropertyDescriptor.Arguments[0],
                parameter),
            _ => throw new QueryEngineException(ErrorCodes.PropertyFilterNotImplementedError,
                $"Type {typeof(TEntity)}, property {filterPropertyDescriptor.Name}, filter {filterPropertyDescriptor.FilterType}" +
                $"is not implemented yet.")
        };
    }
    
    public static BinaryExpression ToBinaryExpression<TEntity>(
        FilterBlockDescriptor blockDescriptor,
        ParameterExpression? existingParameter = null)
    {
        var parameter = existingParameter ?? Expression.Parameter(typeof(TEntity));
        var result = ToBinaryExpression<TEntity>(blockDescriptor.First, parameter);
        
        if (blockDescriptor.IsComposite)
        {
            var condition = blockDescriptor.Condition ?? LogicalOperator.And;
                
            blockDescriptor.Others.ToList().ForEach(item =>
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
