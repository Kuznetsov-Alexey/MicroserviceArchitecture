using CommandsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
    {
        //
    }
    
    public DbSet<Command> Commands { get; set; }
    public DbSet<Platform> Platforms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder
            .Entity<Platform>()
            .HasMany<Command>()
            .WithOne(command => command.Platform)
            .HasForeignKey(command => command.PlatformId);

        modelBuilder
            .Entity<Command>()
            .HasOne<Platform>()
            .WithMany(platform => platform.Commands)
            .HasForeignKey(command => command.PlatformId);
    }
}