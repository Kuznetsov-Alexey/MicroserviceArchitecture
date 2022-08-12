using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.MappingProfiles;

public class PlatformProfile : Profile
{
    public PlatformProfile()
    {
        CreateMap<PlatformCreateDto, Platform>();
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<PlatformReadDto, PlatformPublishedDto>();
    }
}