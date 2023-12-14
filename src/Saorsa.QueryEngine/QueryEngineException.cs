using Saorsa.Exceptions;

namespace Saorsa.QueryEngine;


/// <summary>
/// Business exception used by Query Engine. Carries an error code and meta data about
/// the domain specific error.
/// </summary>
public class QueryEngineException : ErrorCodeException
{
    /// <summary>Initializes a new instance of the class.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errorCode">The unique error code of the exception.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter
    /// is not a null reference, the current exception is raised in a <see langword="catch" /> block
    /// that handles the inner exception.
    /// </param>
    public QueryEngineException(
        int errorCode,
        string? message = null,
        Exception? innerException = null) : base(errorCode, message, innerException)
    {
    }
}
