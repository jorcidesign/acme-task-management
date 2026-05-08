namespace AcmeTaskApi.Application.Common;
 
/// <summary>
/// Represents the outcome of an operation without a return value.
/// Avoids using exceptions for expected control flow (e.g., validation failures).
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
 
    protected Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }
 
    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
}
 
/// <summary>
/// Represents the outcome of an operation with a typed return value.
/// </summary>
public class Result<T> : Result
{
    public T? Value { get; }
 
    private Result(T value) : base(true, null) => Value = value;
    private Result(string error) : base(false, error) { }
 
    public static Result<T> Success(T value) => new(value);
    public static new Result<T> Failure(string error) => new(error);
}
 