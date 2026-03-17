using System.Security.Claims;

namespace DevInsightForge.Application.Abstractions.InternalServices;

public interface ITokenService
{
    (string token, DateTime expiry) GenerateJwtToken(List<Claim> claims);
}




