namespace DevInsightForge.WebAPI.Common.Models;

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; init; }

    internal static ApiResponse<T> SuccessResponse(T? data, int statusCode, params string[] messages) =>
        new()
        {
            Success = true,
            StatusCode = statusCode,
            Message = [.. messages],
            Data = data
        };

    internal static new ApiResponse<T> FailureResponse(int statusCode, IEnumerable<string> messages, string? errorCode = null) =>
        new()
        {
            Success = false,
            StatusCode = statusCode,
            Message = [.. messages],
            ErrorCode = errorCode
        };
}
