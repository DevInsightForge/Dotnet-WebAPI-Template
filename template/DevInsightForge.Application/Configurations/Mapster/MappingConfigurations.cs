using Mapster;

namespace DevInsightForge.Application.Configurations.Mapster;

public static class MappingConfigurations
{
    public static void ConfigureMappings()
    {
        TypeAdapterConfig.GlobalSettings.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible);
    }
}

