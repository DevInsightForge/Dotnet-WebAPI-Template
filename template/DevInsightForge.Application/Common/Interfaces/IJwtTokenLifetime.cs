namespace DevInsightForge.Application.Common.Interfaces;

public interface IJwtTokenLifetime
{
    double AccessTokenExpirationInMinutes { get; }
    double RefreshTokenExpirationInMinutes { get; }
}
