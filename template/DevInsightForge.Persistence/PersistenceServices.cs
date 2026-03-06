using DevInsightForge.Application.Abstructions.DataAccess;
using DevInsightForge.Persistence.DataAccess;
using DevInsightForge.Persistence.DataContext;
using DevInsightForge.Persistence.DataContext.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevInsightForge.Persistence;

public static class PersistenceServices
{
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure DbContext provider
        services.AddDbContext<DatabaseContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<AuditableEntityInterceptor>());
            options.UseNpgsql(configuration.GetConnectionString("DatabaseServer"));
        });

        // Register data-access services
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}
