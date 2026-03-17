using DevInsightForge.Application.Contracts.Common;

namespace DevInsightForge.Application.Abstractions;

public interface IEmailService
{
    Task SendAsync(EmailMessageDto email, CancellationToken cancellationToken = default);
}



