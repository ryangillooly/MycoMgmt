using MycoMgmt.Domain.Models;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.Infrastructure.DataStores.Neo4J;
using MycoMgmt.Infrastructure.Repositories;
using Neo4j.Driver;

namespace MycoMgmt.API.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            
            services.AddTransient<IActionRepository,  ActionRepository>();
            
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