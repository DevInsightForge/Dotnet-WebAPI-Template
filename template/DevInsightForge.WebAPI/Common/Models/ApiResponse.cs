namespace DevInsightForge.WebAPI.Common.Models;

public record ApiResponse
{
    internal ApiResponse() { }
    public required int StatusCode { get; init; }
    public required List<string> Message { get; init; }
}

public sealed record ApiResponse<T> : ApiResponse
    where T : notnull
{
    internal ApiResponse() { }
    public required T Data { get; init; }
}
