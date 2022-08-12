using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/c/platforms/{platformId:int}/[controller]")]
[ApiController]
public class CommandsController : ControllerBase
{
    private readonly ICommandRepo _repository;
    private readonly IMapper _mapper;
    
    public CommandsController(ICommandRepo repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
    {
        Console.WriteLine($"--> Hit {nameof(GetCommandsForPlatform)}");
        
        if (!_repository.PlatformExists(platformId))
            return NotFound();
        
        var commands = _repository
            .GetCommandsForPlatform(platformId)
            .Select(_mapper.Map<CommandReadDto>);

        return Ok(commands);
    }
    
    [HttpGet("{commandId:int}", Name = nameof(GetCommandForPlatform))]
    public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
    {
        Console.WriteLine($"--> Hit {nameof(GetCommandForPlatform)}: {platformId} / {commandId}");

        if (!_repository.PlatformExists(platformId))
            return NotFound();
        
        var command = _repository.GetCommand(platformId, commandId);

        if (command == null)
            return NotFound();

        return Ok(_mapper.Map<CommandReadDto>(command));
    }

    [HttpPost]
    public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
    {
        Console.WriteLine($"--> Hit {nameof(CreateCommandForPlatform)}: {platformId}");

        if (!_repository.PlatformExists(platformId))
            return NotFound();
        
        var command = _mapper.Map<Command>(commandCreateDto);
        _repository.CreateCommand(platformId, command);
        _repository.SaveChanges();

        var commandReadDto = _mapper.Map<CommandReadDto>(command);

        return CreatedAtRoute(nameof(GetCommandForPlatform), new {commandId = command.Id, platformId }, commandReadDto);
    }
}