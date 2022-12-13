﻿using System.Runtime.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MycoMgmt.API.DataStores;
using MycoMgmt.API.DataStores.Neo4J;
using MycoMgmt.API.Models.User_Management;
using MycoMgmt.API.Repositories;
using MycoMgmt.API.Repositories.Recipe;
using Neo4j.Driver;

namespace MycoMgmt.API
{
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
            services.Configure<Neo4JSettings>(Configuration.GetSection("Neo4JSettings"));

            var settings = new Neo4JSettings();
            Configuration.GetSection("Neo4JSettings").Bind(settings);
            
            services.AddSingleton<IDriver>(GraphDatabase.Driver(settings.Neo4jConnection, AuthTokens.Basic(settings.Neo4jUser, settings.Neo4jPassword)));
            services.AddScoped<INeo4JDataAccess, Neo4JDataAccess>();
            services.AddTransient<ICultureRepository, CultureRepository>();
            services.AddTransient<ILocationsRepository, LocationsRepository>();
            services.AddTransient<IStrainsRepository, StrainsRepository>();
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IRecipeRepository, RecipeRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IPermissionRepository, PermissionRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
}