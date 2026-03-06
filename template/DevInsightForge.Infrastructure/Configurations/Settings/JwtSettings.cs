namespace DevInsightForge.Infrastructure.Configurations.Settings;

public class JwtSettings
{
    public string SecretKey { get; set; } = "Default_Super_Secret_256_Bits_Signing_Key";
    public bool ValidateIssuer { get; set; } = false;
    public string ValidIssuer { get; set; } = "DefaultIssuer";
    public bool ValidateAudience { get; set; } = false;
    public string ValidAudience { get; set; } = "DefaultAudience";
    public double AccessTokenExpirationInMinutes { get; set; } = 60;
    public double RefreshTokenExpirationInMinutes { get; set; } = 1440;
}
