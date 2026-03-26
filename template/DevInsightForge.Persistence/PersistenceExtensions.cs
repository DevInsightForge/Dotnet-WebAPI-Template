using DevInsightForge.Application.Abstractions.DataAccess;
using DevInsightForge.Persistence.DataAccess;
using DevInsightForge.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DevInsightForge.Persistence;

public static class PersistenceExtensions
{
    public static void AddPersistence(this IServiceCollection services, Action<DbContextOptionsBuilder>? configureDbContext = null)
    {
        services.AddDbContext<DatabaseContext>((_, options) =>
        {
            if (configureDbContext is null)
            {
                options.UseInMemoryDatabase("DevInsightForgeDb");
                return;
            }

            configureDbContext(options);
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}

