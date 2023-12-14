namespace Saorsa.QueryEngine.Model;


/// <summary>
/// Logical operators are used to perform logical operation such as and, or. Logical operators operates on boolean
/// expressions and returns boolean values (true / false).
/// </summary>
public enum LogicalOperator
{
    /// <summary>
    /// Applies a logical -AND- to the target operands of an expression.
    /// </summary>
    And,

    /// <summary>
    /// Applies a logical -OR- to the target operands of an expression.
    /// </summary>
    Or
}
