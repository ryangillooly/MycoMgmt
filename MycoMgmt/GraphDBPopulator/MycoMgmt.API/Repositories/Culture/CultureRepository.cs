using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MycoMgmt.API.DataStores.Neo4J;
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

        public async Task<List<Dictionary<string, object>>> SearchCulturesByName(string name)
        {
            var query = $"MATCH (c:Culture) WHERE toUpper(c.Name) CONTAINS toUpper('{ name }') RETURN c{{ Name: c.Name, Type: c.Type }} ORDER BY c.Name LIMIT 5";
            
            var persons = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "c");

            return persons;
        }
        
        public async Task<List<Dictionary<string, object>>> GetCultureByName(string name)
        {
            var query = $"MATCH (c:Culture) WHERE toUpper(c.Name) = toUpper('{ name }') RETURN c{{ Name: c.Name, Type: c.Type }} ORDER BY c.Name LIMIT 5";
            
            var persons = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "c");

            return persons;
        }

        public async Task<string> GetCultureById(string id)
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
        
        public async Task<string> AddCulture(Models.Mushrooms.Culture culture)
        {
            if (culture == null || string.IsNullOrWhiteSpace(culture.Name))
                throw new System.ArgumentNullException(nameof(culture), "Culture must not be null");

            try
            {
                var queryList = new List<string>
                {
                    $@"
                        MERGE 
                        (
                            c:Culture
                            {{
                                Name:  '{culture.Name}',
                                Type:  '{culture.Type}'
                            }}        
                        ) 
                        RETURN c;
                        ",
                        $@"
                            MATCH 
                                (c:Culture {{ Name: '{culture.Name}' }}), 
                                (s:Strain {{ Name: '{culture.Strain}' }})
                            MERGE
                                (c)-[r:HAS_STRAIN]->(s)
                            RETURN r",
                        $@"
                            MATCH 
                                (c:Culture  {{ Name: '{culture.Name}' }}), 
                                (l:Location {{ Name: '{culture.Location}' }})
                            MERGE
                                (c)-[r:STORED_IN]->(l)
                            RETURN r
                        "
                    };

                if (culture.Parent != null)
                {
                    queryList.Add(
                        $@"
                                MATCH 
                                    (c:Culture {{ Name: '{culture.Name}' }}), 
                                    (p:Culture {{ Name: '{culture.Parent}' }})
                                MERGE
                                    (c)-[r:HAS_PARENT]->(p)
                                RETURN r
                            ");
                }

                var result = await _neo4JDataAccess.RunTransaction(queryList);

                return result;
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