namespace Saorsa.QueryEngine;

public class QueryEngineException : KeyException<int>
{
    public int ErrorCode => Key;
    
    public QueryEngineException(
        int errorCode, string? message, Exception? innerException) : base(errorCode, message, innerException)
    {
    }
    
    public QueryEngineException(
        int errorCode, string? message) : base(errorCode, message)
    {
    }
    
    public QueryEngineException(int errorCode) : base(errorCode, null)
    {
    }
}
