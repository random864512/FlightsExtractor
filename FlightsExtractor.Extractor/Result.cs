namespace FlightsExtractor.Extractor;

internal class Result
{
    internal static Result<T> Ok<T>(T value) => new(true, value, default);
    internal static Result<T> Error<T>(string error) => new(false, default, error);
}

public record Result<T>
{
    public Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }

    // should be internal, but operators overriding must be defined public
    public static implicit operator Result<T>(T value) => Ok(value);
}