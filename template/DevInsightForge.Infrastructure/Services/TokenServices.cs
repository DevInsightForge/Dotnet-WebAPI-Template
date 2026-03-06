using DevInsightForge.Application.Common.Interfaces;
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

    public (string AccessToken, DateTime AccessTokenExpiresAt) GenerateJwtToken(Guid userId)
    {
        var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationInMinutes);

        var authClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sid, userId.ToString(), ClaimValueTypes.Sid),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        var securityToken = new JwtSecurityToken(
            issuer: _jwtSettings.ValidIssuer,
            audience: _jwtSettings.ValidAudience,
            expires: accessTokenExpiresAt,
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(securityToken);
        return (accessToken, accessTokenExpiresAt);
    }
}
