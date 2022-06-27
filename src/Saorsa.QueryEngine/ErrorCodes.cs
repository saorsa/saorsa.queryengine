namespace Saorsa.QueryEngine;

public static class ErrorCodes
{
    /// <summary>
    /// Error when trying to compile a type definition for a given type.
    /// </summary>
    public const int TypeDefinitionCompilationError = 1100;

    /// <summary>
    /// Error when trying to match property for a target type.
    /// </summary>
    public const int PropertyNotFoundError = 1200;

    /// <summary>
    /// Error when specifying an incompatible filter for a given type property.
    /// </summary>
    public const int PropertyFilterInvalidError = 1300;

    /// <summary>
    /// Error when applying filter, since it is not yet implemented.
    /// </summary>
    public const int PropertyFilterNotImplementedError = 1399;

    /// <summary>
    /// Error when passing a wrong number of arguments for a given property filter.
    /// </summary>
    public const int ArgumentLengthError = 2501;
    
    /// <summary>
    /// Error while performing conversion from a source to target type.
    /// </summary>
    public const int TypeConversionError = 21500;
    
    /// <summary>
    /// Error while performing conversion from a source to target type - expected argument is a literal value,
    /// not an object or array;
    /// </summary>
    public const int TypeConversionLiteralValueExpectedError = 21550;
}
