using DevInsightForge.Application.Results;
using DevInsightForge.WebAPI.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevInsightForge.WebAPI.Common.Mappings;


public static class ApiResponseMapperExtension
{
    private const string OkMessage = "Operation completed successfully.";
    private const string CreatedMessage = "Record created successfully.";
    private const string UnexpectedErrorMessage = "An unexpected error occurred.";

    public static ObjectResult ToApiResponse(this Result result)
    {
        if (result.IsSuccess)
        {
            return ToSuccessResult(result.SuccessType);
        }

        return ToErrorResult(result.Error, result.ValidationError);
    }

    public static ObjectResult ToApiResponse<T>(this Result<T> result)
        where T : notnull
    {
        if (result.IsSuccess)
        {
            return ToSuccessResult(result.SuccessType, result.Value);
        }

        return ToErrorResult<T>(result.Error, result.ValidationError);
    }

    private static ObjectResult ToSuccessResult(SuccessType successType)
    {
        var status = StatusCodeFor(successType);
        var response = new ApiResponse
        {
            StatusCode = status,
            Message = [MessageFor(successType)]
        };

        return new ObjectResult(response) { StatusCode = status };
    }

    private static ObjectResult ToSuccessResult<T>(SuccessType successType, T data)
        where T : notnull
    {
        var status = StatusCodeFor(successType);
        var response = new ApiResponse<T>
        {
            StatusCode = status,
            Message = [MessageFor(successType)],
            Data = data
        };

        return new ObjectResult(response) { StatusCode = status };
    }

    private static ObjectResult ToErrorResult(Error? error, ValidationError? validationError)
    {
        var status = error is not null ? StatusCodeFor(error.Type) : StatusCodes.Status500InternalServerError;
        var messages = error is not null ? ToMessages(error, validationError) : [UnexpectedErrorMessage];
        var response = new ApiResponse
        {
            StatusCode = status,
            Message = messages
        };

        return new ObjectResult(response) { StatusCode = status };
    }

    private static ObjectResult ToErrorResult<T>(Error? error, ValidationError? validationError)
        where T : notnull
    {
        var status = error is not null ? StatusCodeFor(error.Type) : StatusCodes.Status500InternalServerError;
        var messages = error is not null ? ToMessages(error, validationError) : [UnexpectedErrorMessage];
        var response = new ApiResponse
        {
            StatusCode = status,
            Message = messages
        };

        return new ObjectResult(response) { StatusCode = status };
    }

    private static List<string> ToMessages(Error error, ValidationError? validationError)
    {
        if (error.Type != ErrorType.Validation || validationError is null)
        {
            return [error.Message];
        }

        var messages = validationError.Errors
            .SelectMany(field => field.Value.Select(fieldError => $"{field.Key}: {fieldError}"))
            .ToList();

        if (messages.Count == 0)
        {
            messages.Add(validationError.Message);
        }

        return messages;
    }

    private static int StatusCodeFor(SuccessType successType) => successType switch
    {
        SuccessType.Created => StatusCodes.Status201Created,
        _ => StatusCodes.Status200OK
    };

    private static string MessageFor(SuccessType successType) => successType switch
    {
        SuccessType.Created => CreatedMessage,
        _ => OkMessage
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
