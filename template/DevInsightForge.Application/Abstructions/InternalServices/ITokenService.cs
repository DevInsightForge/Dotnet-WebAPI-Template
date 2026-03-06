using System.Security.Claims;

namespace DevInsightForge.Application.Abstructions;

public interface ITokenService
{
    (string token, DateTime expiry) GenerateJwtToken(List<Claim> claims);
}


