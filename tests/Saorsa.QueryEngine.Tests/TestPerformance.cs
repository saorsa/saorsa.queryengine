using NUnit.Framework.Internal;

namespace Saorsa.QueryEngine.Tests;

public class TestPerformance
{
    [Test]
    public void Test()
    {
        var start = DateTimeOffset.Now;
        var typeDef = QueryEngine.BuildTypeDefinition<TestDummyClass>(4);
        var interim = DateTimeOffset.Now;
        var diff1 = interim - start;

        QueryEngine.CompileType<TestDummyClass>(4);

        var start2 = DateTimeOffset.Now;
        var typeDef2 = QueryEngine.GetCompiled<TestDummyClass>(4);
        var diff2 = DateTimeOffset.Now - start2;
        Console.WriteLine(1);
    }
}