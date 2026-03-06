using DevInsightForge.Application.Configurations.Mapster;
using DispatchR.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace DevInsightForge.Application;

public static class ApplicationServices
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        // Register request handlers and mediator
        services.AddDispatchR(typeof(ApplicationServices).Assembly);

        // Register validators from application assembly
        services.AddValidatorsFromAssembly(typeof(ApplicationServices).Assembly);

        // Register mapping profiles
        MappingConfigurations.ConfigureMappings();
    }
}

