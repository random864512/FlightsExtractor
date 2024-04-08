namespace FlightsExtractor.Extractor;

public class Result
{
    public static Result<T> Ok<T>(T value) => new(true, value, default);
    public static Result<T> Error<T>(string error) => new(false, default, error);
}

public class Result<T>(bool isSuccess, T? value, string? error)
{
    public bool IsSuccess { get; } = isSuccess;
    public T? Value { get; } = value;
    public string? Error { get; } = error;

    public Result<E> Map<E>(Func<T, Result<E>> map)
    {
        if (!IsSuccess)
            return Error<E>(Error!);

        return map(Value!);
    }

    public static implicit operator Result<T>(T value) => Ok(value);
}