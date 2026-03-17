using DevInsightForge.Application.Configurations.Mapster;
using DispatchR.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace DevInsightForge.Application;

public static class ApplicationExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddDispatchR(typeof(ApplicationExtensions).Assembly);
        services.AddValidatorsFromAssembly(typeof(ApplicationExtensions).Assembly);
        MappingConfigurations.ConfigureMappings();
    }
}



