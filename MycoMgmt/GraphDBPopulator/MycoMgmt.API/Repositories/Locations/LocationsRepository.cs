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
    public class LocationsRepository : ILocationsRepository
    {
        private readonly INeo4JDataAccess _neo4JDataAccess;
        private ILogger<LocationsRepository> _logger;

        public LocationsRepository(INeo4JDataAccess neo4JDataAccess, ILogger<LocationsRepository> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }
        
        public async Task<string> Add(Location location)
        {
            if (location == null || string.IsNullOrWhiteSpace(location.Name))
                throw new ArgumentNullException(nameof(location), "Location must not be null");

            if ((location.ModifiedBy != null && location.ModifiedOn == null) || (location.ModifiedBy == null && location.ModifiedOn != null))
                throw new ArgumentException("ModifiedBy and ModifiedOn must either both be Null, or both be Populated");
            
            return await PersistToDatabase(location);
        }
        
        public async Task<List<Dictionary<string, object>>> GetAll()
        {
            const string query = @"MATCH (l:Location) RETURN l { Name: l.Name } ORDER BY l.Name";
            var locations = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "l");

            return locations;
        }

        private async Task<string> PersistToDatabase(Location location)
        {
            try
            {
                var queryList = CreateQueryList(location);
                var result = await _neo4JDataAccess.RunTransaction(queryList);
                return result;
            }
            catch (ClientException ex)
            {
                if (!Regex.IsMatch(ex.Message, @"Node\(\d+\) already exists with *"))
                    throw;

                return JsonConvert.SerializeObject(new { Message = $"A culture already exists with the name {location.Name}" });
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        private static List<string> CreateQueryList(Location location)
        {
            var queryList = new List<string>
            {
                $@"
                    MERGE(l:Location {{ Name:  '{location.Name}' }}) 
                    RETURN l;
                    "
            };
            return queryList;
        }
    }
}