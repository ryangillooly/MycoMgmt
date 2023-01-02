using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MycoMgmt.Domain.Models;
using MycoMgmt.Infrastructure.DataStores.Neo4J;
using Neo4j.Driver;
using Newtonsoft.Json;
#pragma warning disable CS8604

namespace MycoMgmt.Infrastructure.Repositories
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
            var queryList = location.CreateQueryList();
            return await _neo4JDataAccess.RunTransaction(queryList);
        }

        public async Task<string> Update(Location location)
        {
            var queryList = location.UpdateQueryList();            
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
        
        public async Task<string> GetAll(Location location, int skip, int limit)
        {
            var result = await _neo4JDataAccess.ExecuteReadListAsync(location.GetAllQuery(skip, limit), "x");
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