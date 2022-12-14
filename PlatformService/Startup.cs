using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

namespace PlatformService;

public class Startup
{
    public IConfiguration Configuration { get; }
    private readonly IWebHostEnvironment _env;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        _env = env;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        if (_env.IsProduction())
        {
            Console.WriteLine("--> Using SqlServer Db");
            services.AddDbContext<AppDbContext>(opt => 
                opt.UseSqlServer(Configuration.GetConnectionString("PlatformsConn")));
        }
        else
        {
            Console.WriteLine("--> Using InMem Db");
            services.AddDbContext<AppDbContext>(opt => 
                opt.UseInMemoryDatabase("InMem"));
        }

        services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
        services.AddSingleton<IMessageBusClient, MessageBusClient>();
        services.AddGrpc();
        services.AddScoped<IPlatformRepository, PlatformRepository>();
        services.AddControllers();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo {Title = "PlatformService", Version = "1.0"});
        });

        Console.WriteLine($"--> CommandService Endpoint {Configuration["CommandService"]}");
    }

    public void Configure(IApplicationBuilder appBuilder, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            appBuilder.UseSwagger();
            appBuilder.UseSwaggerUI();
        }
        
        //appBuilder.UseHttpsRedirection();

        appBuilder.UseRouting();
        appBuilder.UseAuthorization();

        appBuilder.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGrpcService<GrpcPlatformService>();

            endpoints.MapGet("/protos/platforms.proto", async context =>
            {
                await context.Response.WriteAsync(await File.ReadAllTextAsync("Protos/platforms.proto"));
            });
        });
        
        PrepDb.PrepPopulation(appBuilder, env.IsProduction());
    }
}