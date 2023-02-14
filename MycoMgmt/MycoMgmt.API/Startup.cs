using System.Reflection;
using AutoMapper;
using MycoMgmt.API.Helpers;
using MycoMgmt.Core.Contracts;
using MycoMgmt.Core.Contracts.Mushroom;
using MycoMgmt.Core.Models;
using MycoMgmt.Core.Models.Mushrooms;

namespace MycoMgmt.API;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddControllers();
        
        services.AddAutoMapper(cfg =>
        {
            cfg.CreateMap<CreateMushroomRequest, Culture>();
            cfg.CreateMap<UpdateMushroomRequest, Culture>();
            cfg.CreateMap<CreateMushroomRequest, Bulk>();
            cfg.CreateMap<UpdateMushroomRequest, Bulk>();
            cfg.CreateMap<CreateMushroomRequest, Spawn>();
            cfg.CreateMap<UpdateMushroomRequest, Spawn>();
            cfg.CreateMap<CreateMushroomRequest, Fruit>();
            cfg.CreateMap<UpdateMushroomRequest, Fruit>();
            cfg.CreateMap<CreateLocationRequest, Location>();
            cfg.CreateMap<UpdateLocationRequest, Location>();
        }, 
        typeof(Startup)
        );

        services
            .AddSwaggerGen()
            .AddDatabase(Configuration)
            .AddRepositories()
            .AddServices();
    }
        
    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseExceptionHandler("/error");
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapRazorPages();
        });
            
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}