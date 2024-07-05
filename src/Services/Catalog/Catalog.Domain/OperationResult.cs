namespace Catalog.Domain;

public record OperationResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
}

public record OperationResult<TResultType> : OperationResult where TResultType : class
{
    public TResultType? Result { get; init; }
}
