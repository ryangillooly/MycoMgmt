using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MycoMgmt.API.DataStores.Neo4J;
using MycoMgmt.API.Helpers;
using MycoMgmt.Domain.Models;
using Neo4j.Driver;
using Newtonsoft.Json;
#pragma warning disable CS8604

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
        
        public async Task<string> Create(Location location)
        {
            if (location == null || string.IsNullOrWhiteSpace(location.Name))
                throw new ArgumentNullException(nameof(location), "Location must not be null");
            
            var queryList = new List<string>
            {
                location.Create(),
                location.CreateCreatedRelationship(),
                location.CreateCreatedOnRelationship()
            };
            
            var result = await _neo4JDataAccess.RunTransaction(queryList);
            return result;
        }

        public async Task<string> Update(Location location, string elementId)
        {
            var query = $"MATCH (l:Location) WHERE elementId(l) = '{elementId}' ";

            var queryList = new List<string>
            {
                location.UpdateModifiedOnRelationship(elementId),
                location.UpdateModifiedRelationship(elementId)
            };
        
            // Update Name
            if (!string.IsNullOrEmpty(location.Name))
                queryList.Add(query + $"SET l.Name = '{location.Name}' RETURN l");
            
            // Update AgentConfigured
            if (location.AgentConfigured != null)
                queryList.Add(query + $"SET l.AgentConfigured = '{location.AgentConfigured}' RETURN l");
            
            var cultures = await _neo4JDataAccess.RunTransaction(queryList);
            return JsonConvert.SerializeObject(cultures, Formatting.Indented);
        }
        
        public async Task<List<object>> GetAll()
        {
            const string query = @"MATCH (l:Location) RETURN l { Name: l.Name } ORDER BY l.Name";
            var locations = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "l");

            return locations;
        }
    }
}