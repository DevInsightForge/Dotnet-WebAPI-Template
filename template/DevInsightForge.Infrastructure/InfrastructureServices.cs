using DevInsightForge.Application.Common.Interfaces.DataAccess;
using DevInsightForge.Application.Common.Interfaces.DataAccess.Repositories;
using DevInsightForge.Infrastructure.DataAccess;
using DevInsightForge.Infrastructure.DataAccess.Repositories;
using DevInsightForge.Infrastructure.Persistence;
using DevInsightForge.Infrastructure.Persistence.Interceptors;
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

        // Register data-access services
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserTokenRepository, UserTokenRepository>();
    }
}
