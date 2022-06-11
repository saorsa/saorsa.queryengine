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
        var compiledTypDefs =
            QueryEngine.CompileTypeDefinitions(
                TestCase3001);
        
        Assert.That(compiledTypDefs, Is.Not.Empty);

        var match = compiledTypDefs.FirstOrDefault(
            td => td.TypeName.Equals(nameof(TestCompileClass)));
        
        Assert.That(match, Is.Not.Null);
        
        var compiledAgain =
            QueryEngine.CompileTypeDefinitions(
                TestCase3001);
        
        Assert.That(compiledAgain, Is.Empty);
    }
}
