using DevInsightForge.Application.Abstructions;
using DevInsightForge.Infrastructure.Configurations.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DevInsightForge.Infrastructure.Services;

public class TokenServices(IOptions<JwtSettings> jwtSettings) : ITokenService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public (string token, DateTime expiry) GenerateJwtToken(List<Claim> claims)
    {
        var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationInMinutes);
        var tokenClaims = claims.Where(c => c.Type != JwtRegisteredClaimNames.Jti).ToList();
        tokenClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString()));

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        var securityToken = new JwtSecurityToken(
            issuer: _jwtSettings.ValidIssuer,
            audience: _jwtSettings.ValidAudience,
            expires: accessTokenExpiresAt,
            claims: tokenClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(securityToken);
        return (accessToken, accessTokenExpiresAt);
    }
}


