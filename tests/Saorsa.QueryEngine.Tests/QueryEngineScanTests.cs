using System.Reflection;

namespace Saorsa.QueryEngine.Tests;

public class QueryEngineScanTests
{
    [Test]
    public void TestScanningAllAssembliesInAppDomain()
    {
        var resultTypes = QueryEngine.ScanQueryEngineTypes();
        
        Assert.That(resultTypes, Is.Not.Null);
        Assert.That(resultTypes, Contains.Item(typeof(TestCompileClass3001)));
    }
    
    [Test]
    public void TestScanningAllAssembliesInExecutingAssembly()
    {
        var resultTypes = QueryEngine.ScanQueryEngineTypes(
            Assembly.GetExecutingAssembly());
        
        Assert.That(resultTypes, Is.Not.Null);
        Assert.That(resultTypes, Contains.Item(typeof(TestCompileClass3001)));
    }
}