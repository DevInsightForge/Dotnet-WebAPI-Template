using DevInsightForge.Application.Abstructions;
using DevInsightForge.Infrastructure.Configurations;
using DevInsightForge.Infrastructure.ExternalServices;
using DevInsightForge.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevInsightForge.Infrastructure;

public static class InfrastructureServices
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtConfigurations>()
            .Bind(configuration.GetSection("JwtConfigurations"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<ApplicationConfigurations>()
            .Bind(configuration.GetSection("ApplicationConfigurations"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<EmailConfigurations>()
            .Bind(configuration.GetSection("EmailSetting"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<ITokenService, TokenServices>();
        services.AddScoped<IEmailService, EmailService>();
    }
}


