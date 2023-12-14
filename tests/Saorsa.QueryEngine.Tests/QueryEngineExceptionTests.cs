namespace Saorsa.QueryEngine.Tests;

public class QueryEngineExceptionTests
{
    [Test]
    public void TestConstructorWithKeyOnly()
    {
        var val = new Random().Next();
        var exception = new QueryEngineException(val);
        
        Assert.Multiple(() =>
        {
            Assert.That(exception.ErrorCode, Is.EqualTo(val));
            Assert.That(exception.ErrorCode, Is.EqualTo(val));
            Assert.That(exception.InnerException, Is.Null);
        });
    }

    [Test]
    public void TestConstructorWithMessage()
    {
        var val = new Random().Next();
        var msg = Guid.NewGuid().ToString("N");
        var exception = new QueryEngineException(val, msg);
        
        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Is.EqualTo(msg));
            Assert.That(exception.InnerException, Is.Null);
        });
    }

    [Test]
    public void TestConstructorWithInnerException()
    {
        var val = new Random().Next();
        var msg = Guid.NewGuid().ToString("N");
        var exception = new QueryEngineException(val, msg, new ArgumentNullException());
        
        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Is.EqualTo(msg));
            Assert.That(exception.InnerException, Is.Not.Null);
            Assert.That(exception.InnerException is ArgumentNullException);
        });
    }
}
