namespace Saorsa.QueryEngine.Tests;

public class QueryEngineIgnoreSystemTests
{
    [Test]
    public void TestIgnoredTypes()
    {
        AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsQueryEngineIgnored())
            .ToList()
            .ForEach(type =>
        {
            var typeDef = QueryEngine.BuildTypeDefinition(type);
            
            Assert.That(typeDef, Is.Null,
                $"Type '{type}' is marked with '{nameof(QueryEngineIgnoreAttribute)}'. " +
                $"Expected return from '{nameof(QueryEngine.BuildTypeDefinition)}' is null for" +
                "ignored types, unless 'overrideIgnores = true' is used.");
        });
    }
    
    [Test]
    public void TestIgnoredTypesWithOverride()
    {
        AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsQueryEngineIgnored())
            .ToList()
            .ForEach(type =>
            {
                var typeDef = QueryEngine.BuildTypeDefinition(type,
                    overrideIgnores: true);
            
                Assert.That(typeDef, Is.Not.Null,
                    $"Type '{type}' is marked with '{nameof(QueryEngineIgnoreAttribute)}'. " +
                    $"Expected return from '{nameof(QueryEngine.BuildTypeDefinition)}' is non-null for" +
                    $"ignored types when 'overrideIgnores: true' is used.");
            });
    }

    [Test]
    public void TestIgnoredPropertyInfo()
    {
        var properties = typeof(TestIgnoreClass)
            .GetProperties()
            .ToList();
        
        Assert.That(properties, Is.Not.Empty);
        properties.ForEach(p => {
            Assert.Multiple(() =>
            {
                Assert.That(QueryEngine.IsIgnoredByQueryEngine(p), Is.True,
                    $"Type '{typeof(TestIgnoreClass)}', property '{p.Name}' is " +
                    $"marked with '{nameof(QueryEngineIgnoreAttribute)}'. Expected " +
                    $"behaviour is that '{nameof(QueryEngine.IsIgnoredByQueryEngine)}' " +
                    $"for the property returns true in this case.");
            });
        });
    }

    [Test]
    public void TestIgnoredPropertyInfos()
    {
        var typeDef = QueryEngine.BuildTypeDefinition<TestNonIgnoreClassWithIgnoreProperties>();
        
        Assert.Multiple(() =>
        {
            Assert.That(typeDef, Is.Not.Null);
            Assert.That(typeDef!.Properties, Is.Not.Null);
            Assert.That(typeDef.Properties!, Has.Length.EqualTo(1));
        });
    }
    
    [Test]
    public void TestIgnoredPropertyInfosWithOverrideIgnores()
    {
        var typeDef = QueryEngine.BuildTypeDefinition<TestNonIgnoreClassWithIgnoreProperties>(
            overrideIgnores: true);
        
        Assert.Multiple(() =>
        {
            Assert.That(typeDef, Is.Not.Null);
            Assert.That(typeDef!.Properties, Is.Not.Null);
            Assert.That(typeDef.Properties!, Has.Length.EqualTo(2));
        });
    }
}
