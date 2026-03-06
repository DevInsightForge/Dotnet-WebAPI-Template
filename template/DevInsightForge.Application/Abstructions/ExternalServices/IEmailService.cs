namespace DevInsightForge.Application.Abstructions;

public interface IEmailService
{
    Task SendAsync(
        string to,
        string subject,
        string body,
        bool isHtml,
        IEnumerable<string>? cc = null,
        CancellationToken ct = default);
}

