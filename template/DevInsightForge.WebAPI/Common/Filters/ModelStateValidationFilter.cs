using DevInsightForge.WebAPI.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DevInsightForge.WebAPI.Common.Filters;

public sealed class ModelStateValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid)
        {
            return;
        }

        var messages = context.ModelState
            .Where(entry => entry.Value is not null && entry.Value.Errors.Count > 0)
            .SelectMany(entry => entry.Value!.Errors.Select(error =>
            {
                var message = !string.IsNullOrWhiteSpace(error.ErrorMessage)
                    ? error.ErrorMessage
                    : (error.Exception?.Message ?? "Invalid value.");

                return $"{entry.Key}: {message}";
            }))
            .Distinct()
            .ToList();

        messages.Insert(0, "One or more validation errors occurred.");

        var response = new ApiResponse
        {
            StatusCode = StatusCodes.Status422UnprocessableEntity,
            Message = messages
        };

        context.Result = new ObjectResult(response)
        {
            StatusCode = StatusCodes.Status422UnprocessableEntity
        };
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
