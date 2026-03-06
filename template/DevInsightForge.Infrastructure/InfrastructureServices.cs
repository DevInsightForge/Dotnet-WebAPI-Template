using DevInsightForge.Application.Abstructions;
using DevInsightForge.Infrastructure.Configurations.Settings;
using DevInsightForge.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevInsightForge.Infrastructure;

public static class InfrastructureServices
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure token settings
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        // Register infrastructure implementations
        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddScoped<ITokenService, TokenServices>();
    }
}


