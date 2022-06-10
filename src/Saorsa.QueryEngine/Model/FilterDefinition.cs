namespace Saorsa.QueryEngine.Model;

public class FilterDefinition
{
    public static readonly FilterDefinition IS_EMPTY = new(FilterType.IS_EMPTY);
    public static readonly FilterDefinition IS_NOT_EMPTY = new(FilterType.IS_NOT_EMPTY);
    public static readonly FilterDefinition IS_NULL = new(FilterType.IS_NULL);
    public static readonly FilterDefinition IS_NOT_NULL = new(FilterType.IS_NOT_NULL);
    public static readonly FilterDefinition EQ = new(
        FilterType.EQ, "<equal-to>", true);
    public static readonly FilterDefinition NOT_EQ = new(
        FilterType.NOT_EQ, "<not-equal-to>", true);
    public static readonly FilterDefinition LT = new(
        FilterType.LT, "<less-than>", true);
    public static readonly FilterDefinition LT_EQ = new(
        FilterType.LT_EQ, "<less-than-or-equal>", true);
    public static readonly FilterDefinition GT = new(
        FilterType.GT, "<greater-than>", true);
    public static readonly FilterDefinition GT_EQ = new(
        FilterType.GT_EQ, "<greater-than-or-equal>", true);
    public static readonly FilterDefinition RANGE = new(
        FilterType.RANGE, "<greater-than-or-equal>", true, "<less-than-or-equal>", true);
    public static readonly FilterDefinition SEQUENCE = new(
        FilterType.SEQUENCE, "<delimited-list-of-values>", true, "<separator-or-comma>", false);
    public static readonly FilterDefinition CONTAINS = new(
        FilterType.CONTAINS, "<contained-string>", true);
    
    
    public static readonly FilterDefinition[] NumericFilters =
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

    public static readonly FilterDefinition[] TextFilters =
    {
        IS_NULL,
        IS_NOT_NULL,
        EQ,
        NOT_EQ,
        SEQUENCE,
        CONTAINS,
    };
    
    public static readonly FilterDefinition[] EnumFilters =
    {
        IS_NULL,
        IS_NOT_NULL,
        EQ,
        NOT_EQ,
        SEQUENCE,
    };
    
    public static readonly FilterDefinition[] ReferenceFilters =
    {
        IS_NULL,
        IS_NOT_NULL,
    };
    
    public static readonly FilterDefinition[] ArrayFilters =
    {
        IS_EMPTY,
        IS_NOT_EMPTY,
    };
    
    public FilterType FilterType { get; }
    
    public string? Arg1 { get; }
    
    public bool? Arg1Required { get; }
    
    public string? Arg2 { get; }
    
    public bool? Arg2Required { get; }

    public FilterDefinition(
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
}

