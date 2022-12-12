using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MycoMgmt.API.DataStores.Neo4J;
using MycoMgmt.API.Repositories.Recipe;

namespace MycoMgmt.API.Repositories
{
    public class StrainsRepository : IStrainsRepository
    {
        private readonly INeo4JDataAccess _neo4JDataAccess;
        private ILogger<StrainsRepository> _logger;

        public StrainsRepository(INeo4JDataAccess neo4JDataAccess, ILogger<StrainsRepository> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }
        
        public async Task<List<Dictionary<string, object>>> GetAllStrains()
        {
            const string query = @"MATCH (s:Strain) RETURN s { Name: s.Name } ORDER BY s.Name";
            var locations = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "s");

            return locations;
        }
    }
}