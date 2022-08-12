using CommandsService.Models;

namespace CommandsService.Data;

public class CommandRepo : ICommandRepo
{
    private readonly AppDbContext _context;

    public CommandRepo(AppDbContext context)
    {
        _context = context;
    }
    
    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _context.Platforms.AsEnumerable();
    }

    public void CreatePlatform(Platform platform)
    {
        ArgumentNullException.ThrowIfNull(platform);
        _context.Platforms.Add(platform);
    }

    public bool PlatformExists(int platformId)
    {
        return _context.Platforms.Any(platform => platform.Id == platformId);
    }

    public bool ExternalPlatformExists(int externalPlatformId)
    {
        return _context.Platforms.Any(platform => platform.ExternalId == externalPlatformId);
    }

    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
        return _context.Commands
            .Where(command => command.PlatformId == platformId)
            .OrderBy(command => command.Platform.Name);
    }

    public Command GetCommand(int platformId, int commandId)
    {
        return _context.Commands
            .FirstOrDefault(command => command.PlatformId == platformId && command.Id == commandId);
    }

    public void CreateCommand(int platformId, Command command)
    {
        ArgumentNullException.ThrowIfNull(command);

        command.PlatformId = platformId;
        _context.Commands.Add(command);
    }
}