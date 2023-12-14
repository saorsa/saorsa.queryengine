using System.ComponentModel.DataAnnotations;

namespace Saorsa.QueryEngine.Tests.EFCore.Entities;


/// <summary>
/// Simple base for test entities.
/// </summary>
/// <typeparam name="TId">Type of the entity PKey.</typeparam>
public abstract class EntityBase<TId>
{
    /// <summary>
    /// Gets or sets the unique ID (PK) of the entity.
    /// </summary>
    public virtual TId Id { get; set; } = default!;

    /// <summary>
    /// Gets or sets the test case identifier.
    /// </summary>
    [MaxLength(512)]
    public string? TestCaseId { get; set; }

    /// <summary>
    /// Gets or sets the test case sub-identifier.
    /// </summary>
    [MaxLength(512)]
    public string? TestSubCaseId { get; set; }

    /// <summary>
    /// Gets or sets creation timestamp of the record.
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets creation label for the record.
    /// </summary>
    [MaxLength(512)]
    public string? CreatedBy { get; set; }
}
