namespace Saorsa.QueryEngine.Annotations;

[AttributeUsage(
    AttributeTargets.Class
    | AttributeTargets.Struct
    | AttributeTargets.Property
    | AttributeTargets.Enum)]
public class QueryEngineIgnoreAttribute : Attribute
{
}
