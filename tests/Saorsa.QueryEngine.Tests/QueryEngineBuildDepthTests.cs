namespace Saorsa.QueryEngine.Tests;

public class QueryEngineBuildDepthTests
{
    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    public void TestBuildTypeDefinitionWithPositiveDepth(int depth)
    {
        var typeDef = QueryEngine
            .BuildTypeDefinition<TestRecursiveClass>(depth);
        Assert.That(typeDef, depth > 0 ? Is.Not.Null : Is.Null);
        var currentDepth = 1;
        while (currentDepth < depth)
        {
            Assert.That(typeDef!.Properties, Is.Not.Null);
            Assert.That(typeDef.Properties!, Has.Length.EqualTo(1));
            typeDef = typeDef.Properties![0];
            currentDepth++;
        }
    }
}