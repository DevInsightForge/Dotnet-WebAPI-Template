namespace DevInsightForge.Application.Common.Interfaces;

public interface ITokenService
{
    (string AccessToken, DateTime AccessTokenExpiresAt) GenerateJwtToken(Guid userId);
}
