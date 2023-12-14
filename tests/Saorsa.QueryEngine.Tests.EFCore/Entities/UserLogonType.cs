namespace Saorsa.QueryEngine.Tests.EFCore.Entities;


/// <summary>
/// Simple enumeration, demonstrates the usage of enums with Query Engine in EFCore.
/// </summary>
public enum UserLogonType
{
    /// <summary>
    /// Logon via Active Directory
    /// </summary>
    ActiveDirectory,

    /// <summary>
    /// Login via OIDC protocol
    /// </summary>
    Oidc,

    /// <summary>
    /// Login via SAML definition
    /// </summary>
    Saml,
}
