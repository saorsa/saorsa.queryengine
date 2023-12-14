namespace Saorsa.QueryEngine.Annotations;


/// <summary>
/// Marks the target class or structure for compilation by Query Engine.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class QueryEngineCompileAttribute: Attribute
{
}
