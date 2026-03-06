using DevInsightForge.Application.Common.Interfaces;
using DevInsightForge.Application.Common.Interfaces.DataAccess;
using DevInsightForge.Application.Common.Interfaces.DataAccess.Repositories;
using DevInsightForge.Domain.Entities.Core;
using DevInsightForge.Infrastructure.Configurations.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DevInsightForge.Infrastructure.Services;

public class TokenServices(
    IOptions<JwtSettings> jwtSettings,
    IUserTokenRepository userTokenRepository,
    IUnitOfWork unitOfWork) : ITokenService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public string GenerateJwtToken(Guid userId, DateTime? expiryDate)
    {
        expiryDate ??= DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationInMinutes);

        var authClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sid, userId.ToString(), ClaimValueTypes.Sid),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        var securityToken = new JwtSecurityToken(
            issuer: _jwtSettings.ValidIssuer,
            audience: _jwtSettings.ValidAudience,
            expires: expiryDate,
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    public async Task<string> GenerateRefreshTokenAsync(Guid userId, DateTime? expiryDate)
    {
        var refreshExpireDate = expiryDate ?? DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationInMinutes);

        var userToken = UserTokenModel.Create(userId, refreshExpireDate);
        await userTokenRepository.AddAsync(userToken);
        await unitOfWork.SaveChangesAsync();

        return userToken.RefreshToken;
    }

    public async Task<UserTokenModel?> GetValidRefreshToken(string refreshToken)
    {
        return await userTokenRepository.GetWhereAsync(x =>
            x.RefreshToken == refreshToken &&
            !x.IsRevoked &&
            x.ExpiresAt > DateTime.UtcNow);
    }
}
