using AdPlatforms.Application.Mappings;
using AutoMapper;

namespace AdPlatforms.Mappings;

public static class MappingServiceCollectionExtensions
{
    public static void AddMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(ConfigureMapper);
    }

    private static void ConfigureMapper(IMapperConfigurationExpression config)
    {
        config.AllowNullCollections = true;
        config.AddGlobalIgnore("Item");
                
        config.AddProfile<AdvertisingDtoMappingsProfile>();
        config.AddProfile<AdvertisingMappingsProfile>();
    }
    
}