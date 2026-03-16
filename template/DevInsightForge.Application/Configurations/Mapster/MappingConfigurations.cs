using DevInsightForge.Application.DtoModels.User;
using DevInsightForge.Domain.Entities;

namespace DevInsightForge.Application.Configurations.Mapster;

public static class MappingConfigurations
{
    public static void ConfigureMappings()
    {
        TypeAdapterConfig.GlobalSettings.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible);

        TypeAdapterConfig<User, UserResponseModel>
            .NewConfig()
            .Map(dest => dest.UserId, src => src.Id.ToString());
    }
}


