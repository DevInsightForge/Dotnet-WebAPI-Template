using DevInsightForge.Application.DtoModels.User;

namespace DevInsightForge.Application.DtoModels.Authentication;

public sealed class AuthSessionResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; set; }
    public UserResponseModel User { get; set; } = new();
}
