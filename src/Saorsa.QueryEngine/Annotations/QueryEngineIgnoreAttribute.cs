namespace Saorsa.QueryEngine.Annotations;


/// <summary>
/// Marks the target class, struct, enum or property as ignored during Query Engine compilation process.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class
    | AttributeTargets.Struct
    | AttributeTargets.Property
    | AttributeTargets.Enum)]
public class QueryEngineIgnoreAttribute : Attribute
{
}
