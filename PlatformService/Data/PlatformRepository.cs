using PlatformService.Models;

namespace PlatformService.Data;

public class PlatformRepository : IPlatformRepository
{
    private readonly AppDbContext _context;

    public PlatformRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _context.Platforms.ToList();
    }

    public Platform? GetPlatformById(int id)
    {
        var platform = _context.Platforms.FirstOrDefault(platform => platform.Id == id);

        return platform;
    }

    public void CreatePlatform(Platform platform)
    {
        ArgumentNullException.ThrowIfNull(platform);
        _context.Platforms.Add(platform);
    }
}