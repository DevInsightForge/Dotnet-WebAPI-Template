using DevInsightForge.Application.DtoModels.Common;

namespace DevInsightForge.Application.Abstructions;

public interface IEmailService
{
    Task SendAsync(EmailMessageDto email, CancellationToken ct = default);
}
