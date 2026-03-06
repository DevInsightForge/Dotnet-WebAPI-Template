using DevInsightForge.WebAPI.Common.Models;
using Microsoft.AspNetCore.Diagnostics;

namespace DevInsightForge.WebAPI.Extensions;

internal sealed class ExceptionHandlerServiceExtension(ILogger<ExceptionHandlerServiceExtension> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception rawException, CancellationToken ct)
    {
        logger.LogError(rawException, "Unhandled exception while processing {Path}", httpContext.Request.Path);

        const int statusCode = StatusCodes.Status500InternalServerError;
        var apiResponse = ApiResponse.FailureResponse(
            statusCode,
            ["An unexpected error occurred."],
            "server_error");

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(apiResponse, ct);

        return true;
    }
}


