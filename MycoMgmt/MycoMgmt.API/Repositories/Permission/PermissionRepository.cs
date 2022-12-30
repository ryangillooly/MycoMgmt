using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.API.DataStores.Neo4J;
using MycoMgmt.API.Helpers;
using Neo4j.Driver;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace MycoMgmt.API.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly INeo4JDataAccess _neo4JDataAccess;
        private ILogger<PermissionRepository> _logger;
        
        public PermissionRepository(INeo4JDataAccess neo4JDataAccess, ILogger<PermissionRepository> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }
        
        public async Task<string> SearchByName(Permission permission)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(permission.SearchByNameQuery(), "x");
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> GetByName(Permission permission)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(permission.GetByNameQuery(), "x");

            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> GetById(Permission permission)
        {
            var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(permission.GetByIdQuery());
            return JsonConvert.SerializeObject(result);
        }
    
        public async Task<string> GetAll(Permission permission, int skip, int limit)
        {

            
            var result = await _neo4JDataAccess.ExecuteReadListAsync(permission.GetAllQuery(skip, limit), "x");
            return JsonConvert.SerializeObject(result);
        }
        
        public async Task<string> Create(Permission permission)
        {
            var queryList = permission.CreateQueryList();
            return await _neo4JDataAccess.RunTransaction(queryList);
        }
        
        public async Task<string> Update(Permission permission)
        {
            var queryList = permission.UpdateQueryList();
            return await _neo4JDataAccess.RunTransaction(queryList);
        }
        
        public async Task Delete(Permission permission)
        {
            var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(permission.Delete());
        
            if(delete.ElementId == permission.ElementId)
                _logger.LogInformation("Node with elementId {ElementId} was deleted successfully", permission.ElementId);
            else
                _logger.LogWarning("Node with elementId {ElementId} was not deleted, or was not found for deletion", permission.ElementId);
        }
        
    }
}