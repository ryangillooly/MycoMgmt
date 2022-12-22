using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MycoMgmt.API.DataStores.Neo4J;
using MycoMgmt.Domain.Models;
using Neo4j.Driver;
using Newtonsoft.Json;

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
        
        public async Task<string> Add(Strain strain)
        {
            if (strain == null || string.IsNullOrWhiteSpace(strain.Name))
                throw new ArgumentNullException(nameof(strain), "Strain must not be null");

            if ((strain.ModifiedBy != null && strain.ModifiedOn == null) || (strain.ModifiedBy == null && strain.ModifiedOn != null))
                throw new ArgumentException("ModifiedBy and ModifiedOn must either both be Null, or both be Populated");
            
            return await PersistToDatabase(strain);
        }

        private async Task<string> PersistToDatabase(Strain strain)
        {
            try
            {
                var queryList = CreateQueryList(strain);
                var result = await _neo4JDataAccess.RunTransaction(queryList);
                return result;
            }
            catch (ClientException ex)
            {
                if (!Regex.IsMatch(ex.Message, @"Node\(\d+\) already exists with *"))
                    throw;

                return JsonConvert.SerializeObject(new { Message = $"A culture already exists with the name {strain.Name}" });
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        private static List<string> CreateQueryList(Strain strain)
        {
            var queryList = new List<string>
            {
                $@"
                        MERGE 
                        (
                            s:Strain
                            {{
                                Name:  '{strain.Name}',
                                Effects: ""{strain.Effects}""
                            }}        
                        ) 
                        RETURN s;
                    "
            };
            return queryList;
        }

        public async Task<List<object>> GetAll()
        {
            const string query = @"MATCH (s:Strain) RETURN s { Name: s.Name } ORDER BY s.Name";
            var locations = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "s");

            return locations;
        }
    }
}