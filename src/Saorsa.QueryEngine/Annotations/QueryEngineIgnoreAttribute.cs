namespace Saorsa.QueryEngine.Annotations;

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property)]
public class QueryEngineIgnoreAttribute : Attribute
{
}
