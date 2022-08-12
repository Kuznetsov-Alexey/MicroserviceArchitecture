using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory scopeFactory,
        IMapper mapper)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
    }
    
    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.PlatformPublished:
                AddPlatform(message);
                break;
            default:
                break; 
        }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
        Console.WriteLine("--> Determining Event");

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

        return eventType?.Event switch
        {
            "Platform_Published" => EventType.PlatformPublished,
            _ => EventType.Undetermined
        };
    }

    private void AddPlatform(string platformPublishMessage)
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

        var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishMessage);

        try
        {
            var platform = _mapper.Map<Platform>(platformPublishedDto);
            if (!repo.ExternalPlatformExists(platform.ExternalId))
            {
                repo.CreatePlatform(platform);
                repo.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> Platform already exists...");
            }
        }
        catch(Exception exception)
        {
            Console.WriteLine($"--> Could not add Platform to DB: {exception.Message}");
        }
    }
}

enum EventType
{
    PlatformPublished,
    Undetermined
}