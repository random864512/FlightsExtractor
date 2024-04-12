namespace FlightsExtractor.Extractor;

public record Result(bool IsSuccess, string? Error);

public record Result<T> : Result
{
    public Result(bool isSuccess, T? value, string? error) : base(isSuccess, error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public T? Value { get; }

    public static implicit operator Result<T>(T value) => ResultExtensions.Ok(value);

    public static implicit operator T?(Result<T> result) => result.IsSuccess ? result.Value : default;
}

public static class ResultExtensions
{
    internal static Result Ok() => new(true, default);
    internal static Result Error(string error) => new(false, error);
    internal static Result<T> Ok<T>(T value) => new(true, value, default);
    internal static Result<T> Error<T>(string error) => new(false, default, error);
    internal static Result<T> ToResult<T>(this T? optional, string error) where T : class => optional != null ? Ok(optional!) : Error<T>(error);
    internal static Result<T> ToResult<T>(this T? optional, string error) where T : struct => optional != null ? Ok(optional.Value) : Error<T>(error);

    internal static U? MapNullable<T, U>(this T? optional, Func<T, U?> map) => optional != null ? map(optional!) : default;
    internal static Result<E> MapResult<T, E>(this Result<T> result, Func<T, Result<E>> map) => result.IsSuccess ? map(result.Value!) : Error<E>(result.Error!);
}