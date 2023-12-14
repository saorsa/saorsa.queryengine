namespace Saorsa.QueryEngine.Model;


/// <summary>
/// Object that carries description and meta-data about a filter expression - its structure and operands.
/// </summary>
public class FilterDescriptor : IEquatable<FilterDescriptor>
{
    /// <summary>
    /// Filter descriptor for an expression that checks if a target collection entity is empty.
    /// </summary>
    public static readonly FilterDescriptor CollectionIsEmpty = new(
        FilterOperatorType.CollectionIsEmpty,
        description: "An expression that checks if a target collection entity is empty.");

    /// <summary>
    /// Filter descriptor for an expression that checks if a target collection entity is NOT empty.
    /// </summary>
    public static readonly FilterDescriptor CollectionIsNotEmpty = new(
        FilterOperatorType.CollectionIsNotEmpty,
        description: "An expression that checks if a target collection entity is NOT empty.");

    /// <summary>
    /// Filter descriptor for an expression that checks if its target is NULL.
    /// </summary>
    public static readonly FilterDescriptor IsNull = new(
        FilterOperatorType.IsNull,
        description: "An expression that checks if its target is NULL.");

    /// <summary>
    /// Filter descriptor for an expression that checks if ts target is NOT NULL.
    /// </summary>
    public static readonly FilterDescriptor IsNotNull = new(
        FilterOperatorType.IsNotNull,
        description: "An expression that checks if ts target is NOT NULL.");

    /// <summary>
    /// Filter descriptor for an expression that checks if its target is equal to the first argument of the expression.
    /// </summary>
    public static readonly FilterDescriptor EqualTo = new(
        FilterOperatorType.EqualTo, "<value>", true,
        description: "An expression that checks if its target is equal to the first argument of the expression.");

    /// <summary>
    /// Filter descriptor for an expression that checks if its target is NOT equal to the first argument of
    /// the expression.
    /// </summary>
    public static readonly FilterDescriptor NotEqualTo = new(
        FilterOperatorType.NotEqualTo, "<value>", true,
        description: "An expression that checks if its target is NOT equal to the first argument of the expression.");

    /// <summary>
    /// Filter descriptor for an expression that checks if its target is less than the first argument of
    /// the expression.
    /// </summary>
    public static readonly FilterDescriptor LessThan = new(
        FilterOperatorType.LessThan, "<value>", true,
        description: "An expression that checks if its target is less than the first argument of the expression.");

    /// <summary>
    /// Filter descriptor for an expression that checks if its target is less than or equal to the first argument of
    /// the expression.
    /// </summary>
    public static readonly FilterDescriptor LessThanOrEqual = new(
        FilterOperatorType.LessThanOrEqual, "<less-than-or-equal>", true,
        description: "An expression that checks if its target is less than or equal to the first argument " +
                     "of the expression.");

    /// <summary>
    /// Filter descriptor for an expression that checks if its target is greater than the first argument of
    /// the expression.
    /// </summary>
    public static readonly FilterDescriptor GreaterThan = new(
        FilterOperatorType.GreaterThan, "<value>", true,
        description: "An expression that checks if its target is greater than the first argument of the expression.");

    /// <summary>
    /// Filter descriptor for an expression that checks if its target is greater than or equal to the first argument of
    /// the expression.
    /// </summary>
    public static readonly FilterDescriptor GreaterThanOrEqual = new(
        FilterOperatorType.GreaterThanOrEqual, "<greater-than-or-equal>", true,
        description: "An expression that checks if its target is less than or equal to the first argument " +
                     "of the expression.");

    /// <summary>
    /// Filter descriptor for an expression that checks if its target value is within the boundary specified between
    /// the first argument of the expression (MIN) and the second argument of the expression (MAX).
    /// </summary>
    public static readonly FilterDescriptor ValueInRange = new(
        FilterOperatorType.ValueInRange, "<min-value>", true, "<max-value>", true,
        description: "An expression that checks if its target value is within the boundary specified between the " +
                     "first argument of the expression (MIN) and the second argument of the expression (MAX).");

    /// <summary>
    /// Filter descriptor for an expression that checks if its target value is contained in the sequence,
    /// specified in the first argument of the expression (a collection); the second argument of the 
    /// expression, optional, specifies the separator of values used in the first item.
    /// </summary>
    public static readonly FilterDescriptor ValueInSequence = new(
        FilterOperatorType.ValueInSequence, 
        "<delimited-list-of-values>", true, 
        "<separator>", false,
        description: "An expression that checks if its target value is contained in the sequence, " +
                     "specified in the first argument of the expression (a collection); the second argument of the " +
                     "expression, optional, specifies the separator of values used in the first item.");

    /// <summary>
    /// Filter descriptor for an expression that checks if its target contains a substring passed as the first
    /// argument of the expression.
    /// </summary>
    public static readonly FilterDescriptor StringContains = new(
        FilterOperatorType.StringContains, "<substring>", true,
        description: "An expression that checks if its target contains a substring passed as the first " +
                     "argument of the expression");

    /// <summary>
    /// Descriptors for filter expressions that can be applied on target numerics.
    /// </summary>
    public static readonly FilterDescriptor[] NumericFilters =
    {
        IsNull,
        IsNotNull,
        EqualTo,
        NotEqualTo,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        ValueInRange,
        ValueInSequence,
    };

    /// <summary>
    /// Descriptors for filter expressions that can be applied on target text / string-like objects.
    /// </summary>
    public static readonly FilterDescriptor[] TextFilters =
    {
        IsNull,
        IsNotNull,
        EqualTo,
        NotEqualTo,
        ValueInSequence,
        StringContains,
    };
    
    /// <summary>
    /// Descriptors for filter expressions that can be applied on target enums.
    /// </summary>
    public static readonly FilterDescriptor[] EnumFilters =
    {
        IsNull,
        IsNotNull,
        EqualTo,
        NotEqualTo,
        ValueInSequence,
    };

    /// <summary>
    /// Descriptors for filter expressions that can be applied on target reference objects.
    /// </summary>
    public static readonly FilterDescriptor[] ReferenceFilters =
    {
        IsNull,
        IsNotNull,
    };

    /// <summary>
    /// Descriptors for filter expressions that can be applied on target arrays and collections.
    /// </summary>
    public static readonly FilterDescriptor[] ArrayFilters =
    {
        IsNull,
        IsNotNull,
        CollectionIsEmpty,
        CollectionIsNotEmpty,
    };

    /// <summary>
    /// Gets or sets the operator type.
    /// </summary>
    public FilterOperatorType OperatorType { get; }
    
    public string? Arg1 { get; }
    
    public bool? Arg1Required { get; }
    
    public string? Arg2 { get; }
    
    public bool? Arg2Required { get; }

    public string? Description { get; }

    /// <summary>
    /// Creates a new instance of the descriptor class.
    /// </summary>
    /// <param name="operatorType">The type of the filter, a.k.a. filter operator.</param>
    /// <param name="arg1">Text, describing the 1st argument of the filter, if any.</param>
    /// <param name="arg1Required">Value indicating, if the first argument of the filter is required or not.</param>
    /// <param name="arg2">Text, describing the 2nd argument of the filter, if any.</param>
    /// <param name="arg2Required">Value indicating, if the second argument of the filter is required or not.</param>
    /// <param name="description">Optional description text.</param>
    public FilterDescriptor(
        FilterOperatorType operatorType,
        string? arg1 = default,
        bool? arg1Required = default,
        string? arg2 = default,
        bool? arg2Required = default,
        string? description = default)
    {
        OperatorType = operatorType;
        Arg1 = arg1;
        Arg1Required = arg1Required;
        Arg2 = arg2;
        Arg2Required = arg2Required;
        Description = description;
    }

    public override string ToString()
    {
        var arg1RequiredString = Arg1Required.HasValue 
            ? Arg1Required.Value ? " (Required)" : " (Optional)"
            : string.Empty;
        var arg2RequiredString = Arg2Required.HasValue
            ? Arg2Required.Value ? " (Required)" : "(optional)"
            : string.Empty;
        return $"[Filter {OperatorType}, Argument1={Arg1}{arg1RequiredString}, Argument2={Arg2}{arg2RequiredString}]";
    }

    public bool Equals(FilterDescriptor? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return OperatorType == other.OperatorType
               && Arg1 == other.Arg1
               && Arg1Required == other.Arg1Required
               && Arg2 == other.Arg2
               && Arg2Required == other.Arg2Required;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType()
               && Equals((FilterDescriptor) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            (int) OperatorType,
            Arg1,
            Arg1Required,
            Arg2,
            Arg2Required);
    }
}
