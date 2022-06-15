namespace Saorsa.QueryEngine.Tests.EFCore.Entities;

public abstract class EntityBase
{ 
    public Guid? TestCaseId { get; set; }
    public Guid? TestSubCaseId { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
}
