// ReSharper disable ClassNeverInstantiated.Local
using Saorsa.QueryEngine.Annotations;

namespace Saorsa.QueryEngine.Tests;

public class QueryEngineTests
{

  //  [Test]
    public void TestBuildPropertyDefinitionsOnIgnoredType()
    {
        //var results = QueryEngine.BuildTypeDefinition<TestIgnoredClass>();
       // Assert.IsEmpty(results!.Properties);
    }
    
  //  [Test]
    public void TestBuildPropertyDefinitionsOnEnums()
    {
        var results = QueryEngine.BuildTypeDefinition<TestEnum>();
      //  Assert.IsEmpty(results!.Properties);
    }

   // [Test]
    public void TestBuildPropertyDefinitionsOnSimpleTypes()
    {
        QueryEngine.SimpleTypes.ToList().ForEach(t =>
        {
            var results = QueryEngine.BuildTypeDefinition(t);
            Assert.That(results, Is.Not.Null);
            Assert.That(results!.Properties, Is.Not.Null);
            Assert.That(results!.Properties, Is.Empty,
                $"Expected no property defs for simple type {t.Name}");
        });
    }
    
   // [Test]
    public void TestBuildPropertyDefinitionsOnArrayTypes()
    {
        new []
        {
            typeof(int[]),
            typeof(List<string>),
            typeof(Dictionary<string, object>)
        }.ToList().ForEach(t =>
        {
            var results = QueryEngine.BuildTypeDefinition(t);
            Assert.That(results, Is.Empty,
                $"Expected no property defs for arrays and generic enumerations {t.Name}");
        });
    }
    
    [Test]
    public void TestBuildPropertyDefinitionsOnClass()
    {
        var results = QueryEngine.BuildTypeDefinition<TestDummyClass>();
        Assert.IsNotNull(results);
    }
}
