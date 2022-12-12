using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MycoMgmt.API.DataStores.Neo4J;
using MycoMgmt.API.Repositories.Recipe;

namespace MycoMgmt.API.Repositories
{
    public class LocationsRepository : ILocationsRepository
    {
        private readonly INeo4JDataAccess _neo4JDataAccess;
        private ILogger<LocationsRepository> _logger;

        public LocationsRepository(INeo4JDataAccess neo4JDataAccess, ILogger<LocationsRepository> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }
        
        public async Task<List<Dictionary<string, object>>> GetAllLocations()
        {
            const string query = @"MATCH (l:Location) RETURN l { Name: l.Name } ORDER BY l.Name";
            var locations = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "l");

            return locations;
        }
    }
}