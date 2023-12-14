namespace Saorsa.QueryEngine.Model;

/// <summary>
/// Object that carries description and meta data about a filter expression - its structure and operands.
/// </summary>
public class FilterDescriptor : IEquatable<FilterDescriptor>
{
    public static readonly FilterDescriptor IS_EMPTY = new(FilterType.IS_EMPTY);
    public static readonly FilterDescriptor IS_NOT_EMPTY = new(FilterType.IS_NOT_EMPTY);
    public static readonly FilterDescriptor IS_NULL = new(FilterType.IS_NULL);
    public static readonly FilterDescriptor IS_NOT_NULL = new(FilterType.IS_NOT_NULL);
    public static readonly FilterDescriptor EQ = new(
        FilterType.EQ, "<equal-to>", true);
    public static readonly FilterDescriptor NOT_EQ = new(
        FilterType.NOT_EQ, "<not-equal-to>", true);
    public static readonly FilterDescriptor LT = new(
        FilterType.LT, "<less-than>", true);
    public static readonly FilterDescriptor LT_EQ = new(
        FilterType.LT_EQ, "<less-than-or-equal>", true);
    public static readonly FilterDescriptor GT = new(
        FilterType.GT, "<greater-than>", true);
    public static readonly FilterDescriptor GT_EQ = new(
        FilterType.GT_EQ, "<greater-than-or-equal>", true);
    public static readonly FilterDescriptor RANGE = new(
        FilterType.RANGE, "<greater-than-or-equal>", true, "<less-than-or-equal>", true);
    public static readonly FilterDescriptor SEQUENCE = new(
        FilterType.SEQUENCE, "<delimited-list-of-values>", true, "<separator-or-comma>", false);
    public static readonly FilterDescriptor CONTAINS = new(
        FilterType.CONTAINS, "<contained-string>", true);
    
    
    public static readonly FilterDescriptor[] NumericFilters =
    {
        IS_NULL,
        IS_NOT_NULL,
        EQ,
        NOT_EQ,
        LT,
        LT_EQ,
        GT,
        GT_EQ,
        RANGE,
        SEQUENCE,
    };

    public static readonly FilterDescriptor[] TextFilters =
    {
        IS_NULL,
        IS_NOT_NULL,
        EQ,
        NOT_EQ,
        SEQUENCE,
        CONTAINS,
    };
    
    public static readonly FilterDescriptor[] EnumFilters =
    {
        IS_NULL,
        IS_NOT_NULL,
        EQ,
        NOT_EQ,
        SEQUENCE,
    };
    
    public static readonly FilterDescriptor[] ReferenceFilters =
    {
        IS_NULL,
        IS_NOT_NULL,
    };
    
    public static readonly FilterDescriptor[] ArrayFilters =
    {
        IS_NULL,
        IS_NOT_NULL,
        IS_EMPTY,
        IS_NOT_EMPTY,
    };
    
    public FilterType FilterType { get; }
    
    public string? Arg1 { get; }
    
    public bool? Arg1Required { get; }
    
    public string? Arg2 { get; }
    
    public bool? Arg2Required { get; }

    public FilterDescriptor(
        FilterType type,
        string? arg1 = default,
        bool? arg1Required = default,
        string? arg2 = default,
        bool? arg2Required = default)
    {
        FilterType = type;
        Arg1 = arg1;
        Arg1Required = arg1Required;
        Arg2 = arg2;
        Arg2Required = arg2Required;
    }

    public override string ToString()
    {
        var arg1RequiredString = Arg1Required.HasValue 
            ? Arg1Required.Value ? " (Required)" : " (Optional)"
            : string.Empty;
        var arg2RequiredString = Arg2Required.HasValue
            ? Arg2Required.Value ? " (Required)" : "(optional)"
            : string.Empty;
        return $"[Filter {FilterType}, Argument1={Arg1}{arg1RequiredString}, Argument2={Arg2}{arg2RequiredString}]";
    }

    public bool Equals(FilterDescriptor? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return FilterType == other.FilterType
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
            (int) FilterType,
            Arg1,
            Arg1Required,
            Arg2,
            Arg2Required);
    }
}
