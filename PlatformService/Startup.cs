﻿using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PlatformService.Data;

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
        services.AddDbContext<AppDbContext>(opt => 
            opt.UseInMemoryDatabase("InMem"));

        services.AddScoped<IPlatformRepository, PlatformRepository>();
        services.AddControllers();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo {Title = "PlatformService", Version = "1.0"});
        });
    }

    public void Configure(IApplicationBuilder appBuilder, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            appBuilder.UseSwagger();
            appBuilder.UseSwaggerUI();
        }
        
        appBuilder.UseHttpsRedirection();

        appBuilder.UseRouting();
        appBuilder.UseAuthorization();

        appBuilder.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            //endpoints.MapGrpcService<GrpcPlatformService>();

            // endpoints.MapGet("/protos/platforms.proto", async context =>
            // {
            //     await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
            // });
        });
        
        PrepDb.PrepPopulation(appBuilder);
    }
}