using AdPlatforms.Application.Contracts;
using AdPlatforms.Contracts;
using AutoMapper;

namespace AdPlatforms.Mappings;

public class AdvertisingMappingsProfile : Profile
{
    public AdvertisingMappingsProfile()
    {
        CreateMap<AdvertisingDto, AdvertisingResponse>();
    }
}