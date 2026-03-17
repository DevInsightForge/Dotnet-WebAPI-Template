using System.Security.Claims;

namespace DevInsightForge.Application.Abstractions;

public interface ITokenService
{
    (string token, DateTime expiry) GenerateJwtToken(List<Claim> claims);
}




