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
        
        public async Task<string> SearchByName(Strain strain)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(strain.SearchByNameQuery(), "x");
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> GetByName(Strain strain)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(strain.GetByNameQuery(), "x");

            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> GetById(Strain strain)
        {
            var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(strain.GetByIdQuery());
            return JsonConvert.SerializeObject(result);
        }
    
        public async Task<string> GetAll(Strain strain, int skip, int limit)
        {
            var result = await _neo4JDataAccess.ExecuteReadListAsync(strain.GetAllQuery(skip, limit), "x");
            return JsonConvert.SerializeObject(result);
        }
        
        public async Task<string> Create(Strain strain)
        {
            var queryList = strain.CreateQueryList();
            return await _neo4JDataAccess.RunTransaction(queryList);
        }
        
        public async Task<string> Update(Strain strain)
        {
            var queryList = strain.UpdateQueryList();
            var results = await _neo4JDataAccess.RunTransaction(queryList);
            return JsonConvert.SerializeObject(results);
        }
        
        public async Task Delete(Strain strain)
        {
            var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(strain.Delete());
        
            if(delete.ElementId == strain.ElementId)
                _logger.LogInformation("Node with elementId {ElementId} was deleted successfully", strain.ElementId);
            else
                _logger.LogWarning("Node with elementId {ElementId} was not deleted, or was not found for deletion", strain.ElementId);
        }
    }
}