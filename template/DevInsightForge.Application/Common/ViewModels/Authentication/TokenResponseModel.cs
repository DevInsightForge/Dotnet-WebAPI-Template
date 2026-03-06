namespace DevInsightForge.Application.Common.ViewModels.Authentication;

public class TokenResponseModel
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; set; }
}
