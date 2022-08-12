using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/c/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly ICommandRepo _repository;
    private readonly IMapper _mapper;

    public PlatformsController(ICommandRepo repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        Console.WriteLine("--> Getting Platforms from CommandService");
        
        var platforms = _repository
            .GetAllPlatforms()
            .Select(_mapper.Map<PlatformReadDto>);

        return Ok(platforms);
    }

    [HttpPost]
    public ActionResult TestInboundConnection()
    {
        Console.WriteLine("---> Inbound POST # Command Service");
        
        return Ok("Inbound test from Platforms Controller");
    }
}