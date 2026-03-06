using System.ComponentModel.DataAnnotations;

namespace DevInsightForge.Infrastructure.Configurations;

public class EmailConfigurations
{
    [Required]
    [EmailAddress]
    public string EmailFrom { get; set; } = "no-reply@devinsightforge.local";

    [Required]
    public string SmtpHost { get; set; } = "localhost";

    [Range(1, 65535)]
    public int SmtpPort { get; set; } = 25;

    public bool SmtpSSL { get; set; }

    public string SmtpUser { get; set; } = string.Empty;

    public string SmtpPassword { get; set; } = string.Empty;
}
