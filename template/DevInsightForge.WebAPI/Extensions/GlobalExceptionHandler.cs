using DevInsightForge.WebAPI.Contracts;
using Microsoft.AspNetCore.Diagnostics;

namespace DevInsightForge.WebAPI.Extensions;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception while processing {Path}", httpContext.Request.Path);

        const int statusCode = StatusCodes.Status500InternalServerError;
        var errorResponse = new ErrorResponse
        {
            Type = "about:blank",
            Title = "Internal Server Error",
            Status = statusCode,
            Detail = "An unexpected error occurred.",
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);

        return true;
    }
}
