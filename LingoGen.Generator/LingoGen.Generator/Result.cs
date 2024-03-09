namespace LingoGen.Generator;

public class Result<TError> where TError : notnull
{
    // ReSharper disable once InconsistentNaming
    protected readonly TError? _error;

    public bool IsSuccess => _error is null;

    public bool IsFailure => !IsSuccess;

    protected Result(TError? error)
    {
        _error = error;
    }

    public TError UnwrapError()
    {
        if (IsSuccess)
        {
            throw new InvalidOperationException("Tried to unwrap error from a successful result");
        }

        return _error!;
    }

    public static Result<TError> Success() => new(default);

    public static Result<TError> Error(TError error) => new(error);
    
    public static Result<TResult, TError> Success<TResult>(TResult result) => Result<TResult, TError>.Success(result);

    public static Result<TResult, TError> Error<TResult>(TError error) => Result<TResult, TError>.Error(error);
}

public class Result<TResult, TError> : Result<TError> where TError : notnull
{
    private readonly TResult? _result;

    private Result(TResult? result, TError? error) : base(error)
    {
        _result = result;
    }

    public static Result<TResult, TError> Success(TResult result) => new(result, default!);

    public new static Result<TResult, TError> Error(TError error) => new(default!, error);

    public TResult Unwrap()
    {
        if (_result is null)
        {
            throw new InvalidOperationException("Tried to unwrap a failed result");
        }

        return _result;
    }

    public T Match<T>(Func<TResult, T> success, Func<TError, T> failure)
    {
        return _result is not null ? success(_result) : failure(_error!);
    }

    public void Match(Action<TResult> success, Action<TError> failure)
    {
        if (_result is not null)
            success(_result);
        else
            failure(_error!);
    }

    public Result<TNewResult, TError> Map<TNewResult>(Func<TResult, TNewResult> mapper) =>
        _result is not null ? Result<TNewResult, TError>.Success(mapper(_result)) : Result<TNewResult, TError>.Error(_error!);

    public Result<TResult, TNewError> MapError<TNewError>(Func<TError, TNewError> mapper) where TNewError : notnull =>
        _result is not null ? Result<TResult, TNewError>.Success(_result) : Result<TResult, TNewError>.Error(mapper(_error!));
}