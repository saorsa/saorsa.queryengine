using System.Reflection;

namespace Saorsa.QueryEngine.Tests;

public class QueryEngineCompileTests
{
    public const int TestCase3001 = 3001;
    
    [Test]
    public void TestCompileAll()
    {
        var resultTypes =
            QueryEngine.CompileTypeDefinitions(-1);
        
        Assert.That(resultTypes, Is.Empty);
    }
    
    [Test]
    public void TestCompileExecutingAssembly()
    {
        var resultTypes =
            QueryEngine.CompileTypeDefinitions(
                Assembly.GetExecutingAssembly(),
                -1);
        
        Assert.That(resultTypes, Is.Empty);
    }
    
    [Test]
    public void TestCompileTestCase3001()
    {
        var compiledMatch = QueryEngine.GetCompiled<TestCompileClass3001>(TestCase3001);
        Assert.That(compiledMatch, Is.Null);
        
        var isCompiled = QueryEngine.IsCompiled<TestCompileClass3001>(TestCase3001);
        Assert.That(isCompiled, Is.False);
        
        var compiledTypDefs =
            QueryEngine.CompileTypeDefinitions(
                TestCase3001);
        
        Assert.That(compiledTypDefs, Is.Not.Empty);

        var match = compiledTypDefs.FirstOrDefault(
            td => td.TypeName.Equals(nameof(TestCompileClass3001)));
        
        Assert.That(match, Is.Not.Null);
        
        isCompiled = QueryEngine.IsCompiled<TestCompileClass3001>(TestCase3001);
        Assert.That(isCompiled, Is.True);

        var match2 = QueryEngine.GetCompiled<TestCompileClass3001>(TestCase3001);
        
        Assert.That(match2, Is.Not.Null);
        Assert.That(match2, Is.SameAs(match));
    }

    [Test]
    public void TestCompileIgnoredType()
    {
        var typeDef = QueryEngine.CompileType<TestIgnoreEnum>();
        Assert.That(typeDef, Is.Null);
    }
}
