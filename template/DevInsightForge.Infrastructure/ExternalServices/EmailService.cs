using DevInsightForge.Application.Abstructions;
using DevInsightForge.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace DevInsightForge.Infrastructure.ExternalServices;

public class EmailService(IOptions<EmailConfigurations> emailOptions) : IEmailService
{
    private readonly EmailConfigurations _emailConfigurations = emailOptions.Value;

    public async Task SendAsync(
        string to,
        string subject,
        string body,
        bool isHtml,
        IEnumerable<string>? cc = null,
        CancellationToken ct = default)
    {
        using var smtpClient = new SmtpClient
        {
            Host = _emailConfigurations.SmtpHost,
            Port = _emailConfigurations.SmtpPort,
            EnableSsl = _emailConfigurations.SmtpSSL,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false
        };

        if (!string.IsNullOrWhiteSpace(_emailConfigurations.SmtpUser))
        {
            smtpClient.Credentials = new NetworkCredential(
                _emailConfigurations.SmtpUser,
                _emailConfigurations.SmtpPassword);
        }

        using var mailMessage = new MailMessage(_emailConfigurations.EmailFrom, to)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = isHtml
        };

        if (cc is not null)
        {
            foreach (var ccAddress in cc.Where(address => !string.IsNullOrWhiteSpace(address)))
            {
                mailMessage.CC.Add(ccAddress);
            }
        }

        await smtpClient.SendMailAsync(mailMessage, ct);
    }
}

