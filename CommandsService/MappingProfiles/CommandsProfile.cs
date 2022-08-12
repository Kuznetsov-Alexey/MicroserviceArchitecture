using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.MappingProfiles;

public class CommandsProfile : Profile
{
    public CommandsProfile()
    {
        CreateMap<Command, CommandReadDto>();
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<CommandCreateDto, Command>();
    }
}