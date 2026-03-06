using DevInsightForge.Application.Common.Interfaces;
using DevInsightForge.Infrastructure.Configurations.Settings;
using Microsoft.Extensions.Options;

namespace DevInsightForge.Infrastructure.Services;

public class JwtTokenLifetime(IOptions<JwtSettings> jwtSettings) : IJwtTokenLifetime
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public double AccessTokenExpirationInMinutes => _jwtSettings.AccessTokenExpirationInMinutes;
    public double RefreshTokenExpirationInMinutes => _jwtSettings.RefreshTokenExpirationInMinutes;
}
