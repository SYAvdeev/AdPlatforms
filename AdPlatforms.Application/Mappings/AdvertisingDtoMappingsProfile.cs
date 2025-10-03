using AdPlatforms.Application.Contracts;
using AdPlatforms.Domain.Entities;
using AutoMapper;

namespace AdPlatforms.Application.Mappings;

public class AdvertisingDtoMappingsProfile : Profile
{
    public AdvertisingDtoMappingsProfile()
    {
        CreateMap<Advertising, AdvertisingDto>();
    }
}