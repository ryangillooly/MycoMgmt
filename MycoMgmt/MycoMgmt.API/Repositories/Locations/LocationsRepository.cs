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
            var queryList = new List<string?>
            {
                location.Create(),
                location.CreateCreatedRelationship(),
                location.CreateCreatedOnRelationship()
            };
            
            return await _neo4JDataAccess.RunTransaction(queryList);
        }

        public async Task<string> Update(Location location)
        {
            var queryList = new List<string?>
            {
                location.UpdateModifiedOnRelationship(),
                location.UpdateModifiedRelationship(),
                location.UpdateName(),
                location.UpdateAgentConfigured()
            };
            
            var results = await _neo4JDataAccess.RunTransaction(queryList);
            return JsonConvert.SerializeObject(results);
        }
        
        public async Task Delete(Location location)
        {
            var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(location.Delete());
        
            if(delete.ElementId == location.ElementId)
                _logger.LogInformation("Node with elementId {ElementId} was deleted successfully", location.ElementId);
            else
                _logger.LogWarning("Node with elementId {ElementId} was not deleted, or was not found for deletion", location.ElementId);
        }
        
        public async Task<string> GetAll(Location location)
        {
            var result = await _neo4JDataAccess.ExecuteReadListAsync(location.GetAll(), "x");
            return JsonConvert.SerializeObject(result);
        }
        
        public async Task<string> SearchByName(Location location)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(location.SearchByNameQuery(), "x");
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> GetByName(Location location)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(location.GetByNameQuery(), "x");

            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> GetById(Location location)
        {
            var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(location.GetByIdQuery());
            return JsonConvert.SerializeObject(result);
        }
    }
}