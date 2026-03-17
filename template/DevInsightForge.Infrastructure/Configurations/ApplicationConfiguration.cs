using System.ComponentModel.DataAnnotations;

namespace DevInsightForge.Infrastructure.Configurations;

public class ApplicationConfiguration
{
    [Required]
    [MinLength(32)]
    public string EncryptionSecretKey { get; set; } = "Default_Super_Secret_256_Bits_Encryption_Key";
}

