using PlatformService.Models;

namespace PlatformService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder appBuilder)
    {
        using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
        {
            AppDbContext context = serviceScope.ServiceProvider.GetService<AppDbContext>();
            SeedData(context);
        }
    }

    private static void SeedData(AppDbContext context)
    {
        if (context.Platforms.Any())
            return;
        
        Console.WriteLine("Seeding data...");
        context.Platforms.AddRange(
            new Platform{ Name = "DotNet", Publisher = "Msft", Cost = "Free"},
            new Platform{ Name = "Sql", Publisher = "Msft", Cost = "Free"},
            new Platform{ Name = "Kubernates", Publisher = "Cloud Native", Cost = "Free"});

        context.SaveChanges();
    }
}