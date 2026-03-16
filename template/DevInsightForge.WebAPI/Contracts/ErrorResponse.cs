namespace DevInsightForge.WebAPI.Contracts;

public sealed record ErrorResponse
{
    public required string Type { get; init; }
    public required string Title { get; init; }
    public required int Status { get; init; }
    public string? Detail { get; init; }
    public string? Instance { get; init; }
    public IReadOnlyList<string>? Errors { get; init; }
}
