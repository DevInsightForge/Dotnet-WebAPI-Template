using FluentValidation.Results;

namespace DevInsightForge.Application.Results;

public sealed record Result<T> : Result
    where T : notnull
{
    private Result(bool isSuccess, T value, Error? error, ValidationError? validationError = null, SuccessType successType = SuccessType.Ok)
        : base(isSuccess, error, validationError, successType)
    {
        Value = value;
    }

    public T Value { get; }

    internal static Result<T> Success(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new(true, value, null);
    }

    internal static Result<T> Created(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new(true, value, null, successType: SuccessType.Created);
    }

    internal static new Result<T> Failure(Error error) =>
        new(false, default!, error);

    internal static new Result<T> ValidationFailure(ValidationError validationError) =>
        new(false, default!,
            new Error(validationError.Code, validationError.Message, ErrorType.Validation),
            validationError);

    internal static new Result<T> ValidationFailure(ValidationResult validationResult) =>
        ValidationFailure(CreateValidationError(validationResult));
}


