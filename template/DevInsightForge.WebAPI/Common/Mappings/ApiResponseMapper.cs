using DevInsightForge.Application.Results;
using DevInsightForge.WebAPI.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevInsightForge.WebAPI.Common.Mappings;


public static class ApiResponseMapperExtension
{
    public static ObjectResult ToApiResponse(this Result result, string successMessage = "Request completed successfully.", int? successStatusCode = null)
    {
        if (result.IsSuccess)
        {
            var statusCode = successStatusCode ?? ResolveSuccessStatusCode(result.SuccessType);
            return new ObjectResult(ApiResponse.SuccessResponse(statusCode, successMessage))
            {
                StatusCode = statusCode
            };
        }

        return BuildFailureResponse(result.Error, result.ValidationError);
    }

    public static ObjectResult ToApiResponse<T>(this Result<T> result, string successMessage = "Request completed successfully.", int? successStatusCode = null)
    {
        if (result.IsSuccess)
        {
            var statusCode = successStatusCode ?? ResolveSuccessStatusCode(result.SuccessType);
            return new ObjectResult(ApiResponse<T>.SuccessResponse(result.Value, statusCode, successMessage))
            {
                StatusCode = statusCode
            };
        }

        return BuildFailureResponse(result.Error, result.ValidationError);
    }

    private static int ResolveSuccessStatusCode(SuccessType successType) =>
        successType switch
        {
            SuccessType.Created => StatusCodes.Status201Created,
            _ => StatusCodes.Status200OK
        };

    private static ObjectResult BuildFailureResponse(Error? error, ValidationError? validationError)
    {
        if (error is null)
        {
            var apiResponse = ApiResponse.FailureResponse(
                StatusCodes.Status500InternalServerError,
                ["An unexpected error occurred."],
                "server_error");

            return new ObjectResult(apiResponse) { StatusCode = StatusCodes.Status500InternalServerError };
        }

        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status422UnprocessableEntity,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status400BadRequest
        };

        var response = ApiResponse.FailureResponse(
            statusCode,
            BuildFailureMessages(error, validationError),
            error.Code);

        return new ObjectResult(response) { StatusCode = statusCode };
    }

    private static List<string> BuildFailureMessages(Error error, ValidationError? validationError)
    {
        if (error.Type != ErrorType.Validation)
        {
            return [error.Message];
        }

        var messages = new List<string>();
        if (!string.IsNullOrWhiteSpace(validationError?.Message))
        {
            messages.Add(validationError.Message);
        }

        if (validationError?.Errors is null || validationError.Errors.Count == 0)
        {
            if (messages.Count == 0)
            {
                messages.Add(error.Message);
            }

            return messages;
        }

        foreach (var (field, fieldErrors) in validationError.Errors)
        {
            foreach (var fieldError in fieldErrors)
            {
                messages.Add($"{field}: {fieldError}");
            }
        }

        return messages;
    }
}
