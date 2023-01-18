using AutoMapper;
using MycoMgmt.API.Helpers;

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
        services.AddAutoMapper(typeof(Startup));
        /*
        services.AddSingleton(provider => new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new UserProfile(provider.GetService<IUserManager>()));
        }).CreateMapper());
        */
        
        services
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
        }
    }
}