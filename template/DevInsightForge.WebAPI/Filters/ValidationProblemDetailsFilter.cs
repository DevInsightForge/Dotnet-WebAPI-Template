using DevInsightForge.WebAPI.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DevInsightForge.WebAPI.Filters;

public sealed class ValidationProblemDetailsFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid)
        {
            return;
        }

        var errors = context.ModelState
            .Where(entry => entry.Value is not null && entry.Value.Errors.Count > 0)
            .SelectMany(entry => entry.Value!.Errors
                .Select(error => !string.IsNullOrWhiteSpace(error.ErrorMessage)
                    ? $"{entry.Key}: {error.ErrorMessage}"
                    : $"{entry.Key}: {error.Exception?.Message ?? "Invalid value."}"))
            .Distinct()
            .ToList();

        var response = new ErrorResponse
        {
            Type = "urn:devinsightforge:error:validation_error",
            Title = "Validation Error",
            Status = StatusCodes.Status422UnprocessableEntity,
            Detail = "One or more validation errors occurred.",
            Instance = context.HttpContext.Request.Path,
            Errors = errors
        };

        context.Result = new ObjectResult(response)
        {
            StatusCode = StatusCodes.Status422UnprocessableEntity,
            ContentTypes = { "application/problem+json" }
        };
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
