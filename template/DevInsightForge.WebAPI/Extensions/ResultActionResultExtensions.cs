using DevInsightForge.Application.Results;
using DevInsightForge.WebAPI.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace DevInsightForge.WebAPI.Extensions;

public static class ResultActionResultExtensions
{
    public static IActionResult ToCreatedActionResult(this Result result)
    {
        return result.IsSuccess
            ? new StatusCodeResult(StatusCodes.Status201Created)
            : ToProblemActionResult(result.Error, result.ValidationError);
    }

    public static ActionResult<T> ToCreatedActionResult<T>(this Result<T> result)
        where T : notnull
    {
        if (result.IsSuccess)
        {
            return new ObjectResult(result.Value) { StatusCode = StatusCodes.Status201Created };
        }

        return ToProblemObjectResult(result.Error, result.ValidationError);
    }

    public static ActionResult<T> ToOkActionResult<T>(this Result<T> result)
        where T : notnull
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(result.Value);
        }

        return ToProblemObjectResult(result.Error, result.ValidationError);
    }

    public static IActionResult ToNoContentActionResult(this Result result)
    {
        return result.IsSuccess
            ? new NoContentResult()
            : ToProblemActionResult(result.Error, result.ValidationError);
    }

    private static IActionResult ToProblemActionResult(Error? error, ValidationError? validationError)
    {
        return ToProblemObjectResult(error, validationError);
    }

    private static ObjectResult ToProblemObjectResult(Error? error, ValidationError? validationError)
    {
        var response = ToProblemResponse(error, validationError);
        return new ObjectResult(response)
        {
            StatusCode = response.Status,
            ContentTypes = { "application/problem+json" }
        };
    }

    private static ErrorResponse ToProblemResponse(Error? error, ValidationError? validationError)
    {
        if (error is null)
        {
            return new ErrorResponse
            {
                Type = "about:blank",
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred."
            };
        }

        if (error.Type == ErrorType.Validation && validationError is not null)
        {
            var status = StatusCodeFor(error.Type);
            return new ErrorResponse
            {
                Type = $"urn:devinsightforge:error:{validationError.Code}",
                Title = "Validation Error",
                Status = status,
                Detail = validationError.Message,
                Errors = [.. validationError.Errors.SelectMany(entry => entry.Value.Select(message => $"{entry.Key}: {message}"))]
            };
        }

        var genericStatus = StatusCodeFor(error.Type);
        return new ErrorResponse
        {
            Type = $"urn:devinsightforge:error:{error.Code}",
            Title = TitleFor(error.Type),
            Status = genericStatus,
            Detail = error.Message
        };
    }

    private static string TitleFor(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => "Validation Error",
        ErrorType.NotFound => "Not Found",
        ErrorType.Conflict => "Conflict",
        ErrorType.Unauthorized => "Unauthorized",
        ErrorType.Forbidden => "Forbidden",
        _ => "Bad Request"
    };

    private static int StatusCodeFor(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => StatusCodes.Status422UnprocessableEntity,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        _ => StatusCodes.Status400BadRequest
    };
}
