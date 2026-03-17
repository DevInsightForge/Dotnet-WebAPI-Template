using DevInsightForge.Application.Abstractions.DataAccess;
using DevInsightForge.Persistence.DataAccess;
using DevInsightForge.Persistence.DataContext;
using DevInsightForge.Persistence.DataContext.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevInsightForge.Persistence;

public static class PersistenceExtensions
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntityInterceptor>();

        services.AddDbContext<DatabaseContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<AuditableEntityInterceptor>());
            options.UseNpgsql(configuration.GetConnectionString("DatabaseServer"),
                npgsql => npgsql.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName));
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}

