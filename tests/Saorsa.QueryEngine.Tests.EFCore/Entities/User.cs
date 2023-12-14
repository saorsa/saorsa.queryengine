using System.ComponentModel.DataAnnotations;
using Saorsa.QueryEngine.Annotations;

namespace Saorsa.QueryEngine.Tests.EFCore.Entities;


/// <summary>
/// Represents a user account entity.
/// </summary>
[QueryEngineCompile]
public class User : EntityBase<Guid>
{
    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    [MaxLength(256)]
    public string Username { get; set; } = $"user-{Guid.NewGuid():N}";

    /// <summary>
    /// Gets or sets a mocked user password hash.
    /// </summary>
    [MaxLength(256)]
    public string? PasswordHash { get; set; }

    /// <summary>
    /// Gets or sets user gender string.
    /// </summary>
    [MaxLength(128)]
    public string? Gender { get; set; }

    /// <summary>
    /// Gets or sets user age.
    /// </summary>
    public int? Age { get; set; }

    /// <summary>
    /// Gets or sets external ID of the user (e.g. when authenticating via SAML / OIDC).
    /// </summary>
    [MaxLength(1024)]
    public string? ExternalId { get; set; }

    /// <summary>
    /// Gets or sets the user logon type.
    /// </summary>
    public UserLogonType? LatestLogonType { get; set; }

    /// <summary>
    /// Gets or sets the FK id of the department, this user is assigned to.
    /// Optional one-department -to- many-users relationship.
    /// </summary>
    public int? DepartmentId { get; set; }

    /// <summary>
    /// Gets or sets the FK department ref, this user is assigned to.
    /// Optional one-department -to- many-users relationship.
    /// </summary>
    public Department? Department { get; set; }

    /// <summary>
    /// Gets the collection of tags associated with this user.
    /// Many-to-Many relationship.
    /// </summary>
    public ICollection<Tag> Tags { get; } = new List<Tag>();
}
