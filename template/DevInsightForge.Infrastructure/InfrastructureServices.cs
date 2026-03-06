using DevInsightForge.Application.Abstructions;
using DevInsightForge.Infrastructure.Configurations;
using DevInsightForge.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevInsightForge.Infrastructure;

public static class InfrastructureServices
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure and validate JWT settings at startup
        services.AddOptions<JwtConfigurations>()
            .Bind(configuration.GetSection("JwtConfigurations"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Register infrastructure implementations
        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddScoped<ITokenService, TokenServices>();
    }
}


