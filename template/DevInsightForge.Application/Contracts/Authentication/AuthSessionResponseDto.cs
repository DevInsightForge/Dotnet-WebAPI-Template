using DevInsightForge.Application.Contracts.User;

namespace DevInsightForge.Application.Contracts.Authentication;

public sealed class AuthSessionResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; set; }
    public UserResponseDto User { get; set; } = new();
}



