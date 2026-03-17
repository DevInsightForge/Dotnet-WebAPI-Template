using DevInsightForge.Application.Contracts.Common;

namespace DevInsightForge.Application.Abstractions.ExternalServices;

public interface IEmailService
{
    Task SendAsync(EmailMessageDto email, CancellationToken cancellationToken = default);
}



