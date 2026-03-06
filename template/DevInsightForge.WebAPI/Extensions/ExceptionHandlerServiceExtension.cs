using DevInsightForge.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DevInsightForge.WebAPI.Extensions;

internal sealed class ExceptionHandlerServiceExtension : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception rawException, CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails = new()
        {
            Instance = httpContext.Request.Path
        };

        switch (rawException)
        {
            case BadRequestException exception:
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Bad Request";
                problemDetails.Detail = exception.Message.ToString();
                break;

            case NotFoundException exception:
                problemDetails.Status = StatusCodes.Status404NotFound;
                problemDetails.Title = "Not Found";
                problemDetails.Detail = exception.Message.ToString();
                break;

            case FluentValidation.ValidationException exception:
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Validation Failed";
                problemDetails.Detail = "One or more validation errors occurred.";

                problemDetails.Extensions["errors"] = exception.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        group => JsonNamingPolicy.CamelCase.ConvertName(group.Key) ?? group.Key,
                        group => group.Select(e => e.ErrorMessage).ToArray());
                break;

            default:
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Internal Server Error";
                break;
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}

