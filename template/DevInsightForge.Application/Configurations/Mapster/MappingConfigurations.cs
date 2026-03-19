using DevInsightForge.Application.Contracts.User;
using DevInsightForge.Domain.Entities;
using DevInsightForge.Application.Contracts.Role;

namespace DevInsightForge.Application.Configurations.Mapster;

public static class MappingConfigurations
{
    public static void ConfigureMappings()
    {
        TypeAdapterConfig.GlobalSettings.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible);

        TypeAdapterConfig<User, UserResponseDto>
            .NewConfig()
            .Map(dest => dest.UserId, src => src.Id.ToString())
            .Map(dest => dest.RoleId, src => src.RoleId.ToString())
            .Map(dest => dest.RoleName, src => src.Role == null ? string.Empty : src.Role.Name);

        TypeAdapterConfig<Role, RoleResponseDto>
            .NewConfig()
            .Map(dest => dest.RoleId, src => src.Id.ToString());
    }
}





