using DevInsightForge.Application.Abstructions;
using DevInsightForge.Application.Abstructions.DataAccess;
using DevInsightForge.Application.Abstructions.DataAccess.Repositories;
using DevInsightForge.Domain.Entities.Core;
using DevInsightForge.Infrastructure.Configurations.Settings;
using DevInsightForge.Infrastructure.DataAccess;
using DevInsightForge.Infrastructure.DataAccess.Repositories;
using DevInsightForge.Infrastructure.Persistence;
using DevInsightForge.Infrastructure.Persistence.Interceptors;
using DevInsightForge.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevInsightForge.Infrastructure;

public static class InfrastructureServices
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure DbContext provider
        services.AddDbContext<DatabaseContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(configuration.GetConnectionString("DatabaseServer"));
        });

        // Configure DbContext interceptor
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

        // Configure token settings
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        // Register data-access services
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Register infrastructure implementations
        services.AddScoped<IPasswordHasher<UserModel>, PasswordHasher<UserModel>>();
        services.AddScoped<IPasswordHashService, PasswordHashService>();
        services.AddScoped<ITokenService, TokenServices>();
    }
}


