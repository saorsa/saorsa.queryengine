namespace Saorsa.QueryEngine.Tests;

public class QueryEngineBuildDepthTests
{
    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    public void TestBuildTypeDefinition(int depth)
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

    [Test]
    public void TestBuildTypeDefinitionForArray()
    {
        var typeDef = QueryEngine.BuildTypeDefinition<int[]>();
        Assert.That(typeDef, Is.Not.Null);
        Assert.That(typeDef!.Type, Is.EqualTo(QueryEngine.SpecialTypeStringKeys.ArrayOrList));
    }
    
    [Test]
    public void TestBuildTypeDefinitionForEnum()
    {
        var typeDef = QueryEngine.BuildTypeDefinition<TestEnumNoMembers>();
        Assert.That(typeDef, Is.Not.Null);
        Assert.That(typeDef!.Type, Is.EqualTo(QueryEngine.SpecialTypeStringKeys.Enumeration));
    }
    
    [Test]
    public void TestBuildTypeDefinitionForObject()
    {
        var typeDef = QueryEngine.BuildTypeDefinition<TestStructure>();
        Assert.That(typeDef, Is.Not.Null);
        Assert.That(typeDef!.Type, Is.EqualTo(QueryEngine.SpecialTypeStringKeys.Object));
    }

    [Test]
    public void TestSimpleTypes()
    {
        QueryEngine.SimpleTypes.ToList().ForEach(t => {
            var typeDef = QueryEngine.BuildTypeDefinition(t);
            Assert.Multiple(() =>
            {
                Assert.That(typeDef, Is.Not.Null);
                Assert.That(QueryEngine.SimpleTypesStringMap[t], Is.EqualTo(typeDef!.Type),
                    $"Type '{t}' is a simple type and is expected to be configured in " +
                    $"The '{nameof(QueryEngine.SimpleTypesStringMap)}' map.");
            });
        });
    }
}
