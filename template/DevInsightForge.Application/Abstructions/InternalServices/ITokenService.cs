namespace DevInsightForge.Application.Abstructions;

public interface ITokenService
{
    (string AccessToken, DateTime AccessTokenExpiresAt) GenerateJwtToken(Guid userId);
}


