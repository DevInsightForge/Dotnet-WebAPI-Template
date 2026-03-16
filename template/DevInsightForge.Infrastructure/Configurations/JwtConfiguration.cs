using System.ComponentModel.DataAnnotations;

namespace DevInsightForge.Infrastructure.Configurations;

public class JwtConfiguration
{
    [Required]
    [MinLength(32)]
    public string SecretKey { get; set; } = "Default_Super_Secret_256_Bits_Signing_Key";
    public bool ValidateIssuer { get; set; } = false;

    [Required]
    public string ValidIssuer { get; set; } = "DefaultIssuer";

    public bool ValidateAudience { get; set; } = false;

    [Required]
    public string ValidAudience { get; set; } = "DefaultAudience";

    [Range(5, 60 * 24)]
    public double AccessTokenExpirationInMinutes { get; set; } = 60;
}
