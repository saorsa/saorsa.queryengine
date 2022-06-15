namespace Saorsa.QueryEngine.API.Controllers;

public class ResultRef<T>
{
    public ResultStatus? Status { get; set; }
    
    public string? Message { get; set; }
    
    public int? Code { get; set; }
    
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;

    public Guid RefId { get; set; } = Guid.NewGuid();
    
    public object? Context { get; set; }
    
    public object? RefType { get; set; }
    
    public T? Result { get; set; }
}

public class ResultRef : ResultRef<object>
{
}