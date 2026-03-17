using FluentValidation.Results;

namespace DevInsightForge.Application.Results;

public record Result
{
    protected Result(bool isSuccess, Error? error, ValidationError? validationError = null, SuccessType successType = SuccessType.Ok)
    {
        IsSuccess = isSuccess;
        Error = error;
        ValidationError = validationError;
        SuccessType = successType;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public SuccessType SuccessType { get; }
    public Error? Error { get; }
    public ValidationError? ValidationError { get; }

    internal static Result Success() => new(true, null);

    internal static Result Created() => new(true, null, successType: SuccessType.Created);

    internal static Result Failure(Error error) => new(false, error);

    internal static Result ValidationFailure(ValidationError validationError) =>
        new(false, new Error(validationError.Code, validationError.Message, ErrorType.Validation), validationError);

    internal static Result ValidationFailure(ValidationResult validationResult) =>
        ValidationFailure(CreateValidationError(validationResult));

    protected static ValidationError CreateValidationError(ValidationResult validationResult)
    {
        var errors = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).Distinct().ToArray());

        return new ValidationError(
            "validation_error",
            "One or more validation errors occurred.",
            errors);
    }
}


