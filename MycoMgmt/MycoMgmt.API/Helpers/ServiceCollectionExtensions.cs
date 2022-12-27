using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MycoMgmt.API.DataStores;
using MycoMgmt.API.DataStores.Neo4J;
using MycoMgmt.API.Repositories;
using MycoMgmt.DataStores.Neo4J;
using MycoMgmt.Domain.Models.Mushrooms;
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
            services.AddTransient<ISpawnRepository,      SpawnRepository>();
            services.AddTransient<IBulkRepository,       BulkRepository>();
            services.AddTransient<IFruitRepository,      FruitRepository>();
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
            
            services.AddSingleton<IDriver>(GraphDatabase.Driver(settings.Neo4JConnection, AuthTokens.Basic(settings.Neo4JUser, settings.Neo4JPassword)));
            services.AddScoped<INeo4JDataAccess, Neo4JDataAccess>();
            
            return services;
        }
    }
}