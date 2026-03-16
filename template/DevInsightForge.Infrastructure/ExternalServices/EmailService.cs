using DevInsightForge.Application.Abstructions;
using DevInsightForge.Application.DtoModels.Common;
using DevInsightForge.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace DevInsightForge.Infrastructure.ExternalServices;

public class EmailService(IOptions<EmailConfiguration> emailOptions) : IEmailService
{
    private readonly EmailConfiguration _emailConfigurations = emailOptions.Value;

    public async Task SendAsync(EmailMessageDto email, CancellationToken ct = default)
    {
        using var smtpClient = new SmtpClient
        {
            Host = _emailConfigurations.SmtpHost,
            Port = _emailConfigurations.SmtpPort,
            EnableSsl = _emailConfigurations.SmtpSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false
        };

        if (!string.IsNullOrWhiteSpace(_emailConfigurations.SmtpUser))
        {
            smtpClient.Credentials = new NetworkCredential(
                _emailConfigurations.SmtpUser,
                _emailConfigurations.SmtpPassword);
        }

        using var mailMessage = new MailMessage(_emailConfigurations.EmailFrom, email.To)
        {
            Subject = email.Subject,
            Body = email.Body,
            IsBodyHtml = email.IsHtml
        };

        if (email.Cc is not null)
        {
            foreach (var ccAddress in email.Cc.Where(address => !string.IsNullOrWhiteSpace(address)))
            {
                mailMessage.CC.Add(ccAddress);
            }
        }

        await smtpClient.SendMailAsync(mailMessage, ct);
    }
}
