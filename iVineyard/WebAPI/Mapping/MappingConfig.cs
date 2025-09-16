using Mapster;
using Model.Configurations;

namespace WebAPI.Mapping;

public class MappingConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<ApplicationUser, ApplicationUser>.NewConfig()
            .PreserveReference(false);
    }
}