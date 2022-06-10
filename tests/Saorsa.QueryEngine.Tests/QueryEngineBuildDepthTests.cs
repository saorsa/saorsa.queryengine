namespace Saorsa.QueryEngine.Tests;

public class QueryEngineBuildDepthTests
{
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    public void TestBuildTypeDefinitionWithDepth(int depth)
    {
        var typeDef = QueryEngine.BuildTypeDefinition<TestRecursiveClass>(depth);
        Assert.That(typeDef, Is.Not.Null);
        var currentDepth = 0;
        while (currentDepth < depth - 1)
        {
            Assert.That(typeDef!.Properties, Is.Not.Null);
            Assert.That(typeDef.Properties!, Has.Length.EqualTo(1));
            typeDef = typeDef.Properties![0];
            currentDepth++;
        }
    }
}