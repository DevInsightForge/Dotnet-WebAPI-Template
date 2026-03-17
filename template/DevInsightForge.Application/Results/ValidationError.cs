namespace DevInsightForge.Application.Results;

public sealed record ValidationError(
    string Code,
    string Message,
    IReadOnlyDictionary<string, string[]> Errors);


