namespace Saorsa.QueryEngine.Model;


/// <summary>
/// Enumeration with the filter operators types supported by Query Engine.
/// </summary>
public enum FilterOperatorType
{
    /// <summary>
    /// Checks if the left operand is NULL
    /// </summary>
    IsNull,

    /// <summary>
    /// Checks if the left operand is NOT NULL
    /// </summary>
    IsNotNull,

    /// <summary>
    /// Checks if the left operand is equal to the right operand.
    /// </summary>
    EqualTo,

    /// <summary>
    /// Checks if the left operand is NOT equal to the right operand.
    /// </summary>
    NotEqualTo,

    /// <summary>
    /// Checks if the left operand is less than the right operand.
    /// </summary>
    LessThan,

    /// <summary>
    /// Checks if the left operand is less than or equal to the right operand.
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// Checks if the right operand is greater than the right operand.
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Checks if the left operand is greater than or equal to the right operand.
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// Checks if the left operand matches a value within the min / max specified as right operands.
    /// </summary>
    ValueInRange,

    /// <summary>
    /// Checks if the left operand matches a value within a collection of allowed values (sequence).
    /// </summary>
    ValueInSequence,

    /// <summary>
    /// Checks if the left string operand contains a value specified as a right operand.
    /// </summary>
    StringContains,

    /// <summary>
    /// Checks if the left operand is an empty collection.
    /// </summary>
    CollectionIsEmpty,

    /// <summary>
    /// Checks if the left operand is NOT an empty collection.
    /// </summary>
    CollectionIsNotEmpty,
}
