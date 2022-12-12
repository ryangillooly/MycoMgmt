using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MycoMgmt.API.DataStores;
using MycoMgmt.API.Models;
using MycoMgmt.Populator.Models;
using Neo4j.Driver;
using Newtonsoft.Json;

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

        public async Task<string> SearchCultureById(string id)
        {
            var query = $"MATCH (c:Culture) WHERE ID(c) = { id } RETURN c";

            try
            {
                var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(query);
                return JsonConvert.SerializeObject(result);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message != "The result is empty.") 
                    throw;
                
                return JsonConvert.SerializeObject(new { Message = $"No results were found for Culture Id { id }" });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        
        public async Task<string> AddCulture(Culture culture)
        {
            if (culture == null || string.IsNullOrWhiteSpace(culture.Name))
                throw new System.ArgumentNullException(nameof(culture), "Culture must not be null");

            try
            {
                var query = $@"
                            MERGE 
                            (
                                c:Culture
                                {{
                                    Name:  '{culture.Name}',
                                    Type:  '{culture.Type}'
                                }}        
                            ) 
                            RETURN c;
                        ";
                
                var strain = $@"
                                    MATCH 
                                        (c:Culture {{ Name: '{culture.Name}' }}), 
                                        (s:Strain {{ Name: '{culture.Strain}' }})
                                    MERGE
                                        (c)-[r:HAS_STRAIN]->(s)
                                    RETURN r";
                
               var location = $@"
                                    MATCH 
                                        (c:Culture  {{ Name: '{culture.Name}' }}), 
                                        (l:Location {{ Name: '{culture.Location}' }})
                                    MERGE
                                        (c)-[r:STORED_IN]->(l)
                                    RETURN r
                                  ";

                var result           = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(query);
                var strainRelResults = await _neo4JDataAccess.ExecuteWriteTransactionAsync<IRelationship>(strain);
                var locationResults  = await _neo4JDataAccess.ExecuteWriteTransactionAsync<IRelationship>(location);
                
                var obj = new
                {
                    culture     = result,
                    strainRel   = strainRelResults,
                    locationRel = locationResults
                };
                
                return JsonConvert.SerializeObject(obj);
            }
            catch (ClientException ex)
            {
                if (!Regex.IsMatch(ex.Message, @"Node\(\d+\) already exists with *"))
                    throw;
                
                return JsonConvert.SerializeObject(new { Message = $"A culture already exists with the name { culture.Name }" });
            }
            catch (Exception ex)
            {
                throw new System.ArgumentException(ex.Message);
            }

            return null;
        }
        
        public async Task<long> GetCultureCount()
        {
            const string query = @"Match (c:Culture) RETURN count(c) as CultureCount";
            var count = await _neo4JDataAccess.ExecuteReadScalarAsync<long>(query);
            return count;
        }
    }
}