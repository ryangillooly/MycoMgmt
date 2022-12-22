using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MycoMgmt.API.DataStores;
using MycoMgmt.API.DataStores.Neo4J;
using MycoMgmt.API.Repositories;
using MycoMgmt.API.Repositories.Recipe;
using MycoMgmt.DataStores.Neo4J;
using Neo4j.Driver;

namespace MycoMgmt.API.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            
            services.AddTransient<ICultureRepository,    CultureRepository>();
            services.AddTransient<ILocationsRepository,  LocationsRepository>();
            services.AddTransient<IStrainsRepository,    StrainsRepository>();
            services.AddTransient<IAccountRepository,    AccountRepository>();
            services.AddTransient<IRecipeRepository,     RecipeRepository>();
            services.AddTransient<IUserRepository,       UserRepository>();
            services.AddTransient<IPermissionRepository, PermissionRepository>();
            services.AddTransient<IRoleRepository,       RoleRepository>();
            
            return services;
        }
        
        public static IServiceCollection AddNeoDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            
            services.Configure<Neo4JSettings>(configuration.GetSection("Neo4JSettings"));
            
            var settings = new Neo4JSettings();
            configuration.GetSection("Neo4JSettings").Bind(settings);
            
            services.AddSingleton<IDriver>(GraphDatabase.Driver(settings.Neo4jConnection, AuthTokens.Basic(settings.Neo4jUser, settings.Neo4jPassword)));
            services.AddScoped<INeo4JDataAccess, Neo4JDataAccess>();
            
            return services;
        }
    }
}