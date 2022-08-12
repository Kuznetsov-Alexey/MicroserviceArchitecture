using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices;

namespace DefaultNamespace;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepository _platformRepository;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClient;
    private readonly IMessageBusClient _messageBusClient;

    public PlatformsController(IPlatformRepository platformRepository, 
        IMapper mapper, 
        ICommandDataClient commandDataClient, 
        IMessageBusClient messageBusClient)
    {
        _platformRepository = platformRepository;
        _mapper = mapper;
        _commandDataClient = commandDataClient;
        _messageBusClient = messageBusClient;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        var platforms = _platformRepository.GetAllPlatforms()
            .Select(_mapper.Map<PlatformReadDto>);

        return Ok(platforms);
    }

    [HttpGet("{id:int}", Name = "GetPlatformById")]
    public ActionResult<PlatformReadDto> GetPlatformById(int id)
    {
        var platform = _platformRepository.GetPlatformById(id);

        if (platform == null)
            return NotFound();

        return Ok(_mapper.Map<PlatformReadDto>(platform));
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
    {
        var platformModel = _mapper.Map<Platform>(platformCreateDto);
        _platformRepository.CreatePlatform(platformModel);
        _platformRepository.SaveChanges();

        var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

        // Send Sync Message 
        try
        {
            await _commandDataClient.SendPlatformToCommandAsync(platformReadDto);
        }
        catch(Exception exception)
        {
            Console.WriteLine($"--> Could not send synchronously: {exception.Message}");
        }
        
        // Send Async Message
        try
        {
            var platformPublishDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
            platformPublishDto.Event = "Platform_Published";
            _messageBusClient.PublishNewPlatform(platformPublishDto);
        }
        catch(Exception exception)
        {
            Console.WriteLine($"--> Could not send asynchronously: {exception.Message}");
        }

        return CreatedAtRoute(nameof(GetPlatformById), new { platformReadDto.Id }, platformReadDto);
    }
}