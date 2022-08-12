using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder applicationBuilder)
    {
        using var scope = applicationBuilder.ApplicationServices.CreateScope();

        var grpcClient = scope.ServiceProvider.GetRequiredService<IPlatformDataClient>();

        var platforms = grpcClient.ReturnAllPlatforms();

        SeedData(scope.ServiceProvider.GetRequiredService<ICommandRepo>(), platforms);
    }

    private static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
    {
        Console.WriteLine("--> Seeding new platforms...");
        
        foreach (var platform in platforms)
        {
            if (repo.ExternalPlatformExists(platform.ExternalId))
                continue;
            
            repo.CreatePlatform(platform);
        }

        repo.SaveChanges();
    }
}