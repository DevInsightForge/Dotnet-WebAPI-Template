namespace DevInsightForge.WebAPI.Common.Models;

public class ApiResponse
{
    public bool Success { get; init; }
    public int StatusCode { get; init; }
    public List<string> Message { get; init; } = [];
    public string? ErrorCode { get; init; }

    internal static ApiResponse SuccessResponse(int statusCode, params string[] messages) =>
        new()
        {
            Success = true,
            StatusCode = statusCode,
            Message = [.. messages]
        };

    internal static ApiResponse FailureResponse(int statusCode, IEnumerable<string> messages, string? errorCode = null) =>
        new()
        {
            Success = false,
            StatusCode = statusCode,
            Message = [.. messages],
            ErrorCode = errorCode
        };
}
