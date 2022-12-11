using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MycoMgmt.API.DataStores;
using MycoMgmt.API.Models;
using MycoMgmt.Populator.Models;
using Neo4j.Driver;

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

        public async Task<INode> SearchCultureById(string id)
        {
            var query = $"MATCH (c:Culture) WHERE ID(c) = { id } RETURN c";
            var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(query);

            return result;
        }
        
        public async Task<INode> AddCulture(Culture culture)
        {
            if (culture != null && !string.IsNullOrWhiteSpace(culture.Name))
            {
                var query = $@"
                                MERGE 
                                (
                                    c:Culture
                                    {{
                                        Name:  '{ culture.Name }',
                                        Type:  '{ culture.Type }'
                                    }}        
                                ) 
                                RETURN c
                            ";

                 var result = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(query);
                 return result;
            }
            else
            {
                throw new System.ArgumentNullException(nameof(culture), "Culture must not be null");
            }
        }
        
        public async Task<long> GetCultureCount()
        {
            const string query = @"Match (c:Culture) RETURN count(c) as CultureCount";
            var count = await _neo4JDataAccess.ExecuteReadScalarAsync<long>(query);
            return count;
        }
    }
}