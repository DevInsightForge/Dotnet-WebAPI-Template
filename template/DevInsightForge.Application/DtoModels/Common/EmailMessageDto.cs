namespace DevInsightForge.Application.DtoModels.Common;

public sealed class EmailMessageDto
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; }
    public IEnumerable<string>? Cc { get; set; }
}
