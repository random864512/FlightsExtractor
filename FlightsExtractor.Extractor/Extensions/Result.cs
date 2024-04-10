using System.Diagnostics;

namespace FlightsExtractor.Extractor;

public class Result
{
    public static Result<T> Ok<T>(T value) => new(true, value, default);
    public static Result<T> Error<T>(string error) => new(false, default, error);
}

public record Result<T>
{
    protected internal Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }

    public static implicit operator Result<T>(T value) => Ok(value);
}