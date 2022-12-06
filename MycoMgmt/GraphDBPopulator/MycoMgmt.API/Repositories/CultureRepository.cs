using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MycoMgmt.API.DataStores;
using MycoMgmt.API.Models;
using MycoMgmt.Populator.Models;

namespace MycoMgmt.API.Repositories
{
    public class CultureRepository : ICultureRepository
    {
        private readonly INeo4JDataAccess _neo4JDataAccess;
        private ILogger<CultureRepository> _logger;
        
        public CultureRepository(INeo4JDataAccess neo4JDataAccess, ILogger<CultureRepository> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }

        public async Task<List<Dictionary<string, object>>> SearchCultureByName(string searchString)
        {
            const string query = @"MATCH (r:Recipe) WHERE toUpper(r.name) CONTAINS toUpper($searchString) RETURN r{ name: r.name, type: r.type } ORDER BY r.Name LIMIT 5";

            IDictionary<string, object> parameters = new Dictionary<string, object> { { "searchString", searchString } };

            var persons = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "p", parameters);

            return persons;
        }
        
        public async Task<bool> AddCulture(Culture culture)
        {
            if (culture != null && !string.IsNullOrWhiteSpace(culture.Name))
            {
                var query = $@"
                                CREATE 
                                (
                                    :Recipe
                                    {{
                                        Id:    '{ culture.Id          }',
                                        Name:  '{ culture.Name        }',
                                        Type:  '{ culture.Type        }',
                                    }}        
                                ) 
                            ";

                 return await _neo4JDataAccess.ExecuteWriteTransactionAsync<bool>(query);
            }
            else
            {
                throw new System.ArgumentNullException(nameof(culture), "Recipe must not be null");
            }
        }
        
        public async Task<long> GetCultureCount()
        {
            const string query = @"Match (r:Recipe) RETURN count(r) as recipeCount";
            var count = await _neo4JDataAccess.ExecuteReadScalarAsync<long>(query);
            return count;
        }
    }
}