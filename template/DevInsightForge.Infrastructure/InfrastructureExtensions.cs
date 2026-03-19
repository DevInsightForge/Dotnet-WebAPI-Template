using DevInsightForge.Application.Abstractions.ExternalServices;
using DevInsightForge.Application.Abstractions.InternalServices;
using DevInsightForge.Infrastructure.Configurations;
using DevInsightForge.Infrastructure.ExternalServices;
using DevInsightForge.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevInsightForge.Infrastructure;

public static class InfrastructureExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtConfiguration>()
            .Bind(configuration.GetSection("JwtConfiguration"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<EmailConfiguration>()
            .Bind(configuration.GetSection("EmailConfiguration"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailService, EmailService>();
    }
}



