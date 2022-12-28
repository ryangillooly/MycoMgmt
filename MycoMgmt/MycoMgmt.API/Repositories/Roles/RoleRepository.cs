using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MycoMgmt.API.DataStores.Neo4J;
using MycoMgmt.API.Helpers;
using MycoMgmt.Domain.Models.UserManagement;
using Neo4j.Driver;
using Newtonsoft.Json;
#pragma warning disable CS8604

// ReSharper disable once CheckNamespace
namespace MycoMgmt.API.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly INeo4JDataAccess _neo4JDataAccess;
        private ILogger<RoleRepository> _logger;
        
        public RoleRepository(INeo4JDataAccess neo4JDataAccess, ILogger<RoleRepository> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }
        
        public async Task<string> SearchByName(IamRole role)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(role.SearchByNameQuery(), "x");
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> GetByName(IamRole role)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(role.GetByNameQuery(), "x");

            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> GetById(IamRole role)
        {
            var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(role.GetByIdQuery());
            return JsonConvert.SerializeObject(result);
        }
    
        public async Task<string> GetAll(IamRole role)
        {
            var result = await _neo4JDataAccess.ExecuteReadListAsync(role.GetAll(), "x");
            return JsonConvert.SerializeObject(result);
        }
        
        public async Task<string> Create(IamRole role)
        {
            var queryList = new List<string?>
            {
                role.Create(),
                role.CreatePermissionRelationship(),
                role.CreateCreatedRelationship(),
                role.CreateCreatedOnRelationship(),
            };

            return await _neo4JDataAccess.RunTransaction(queryList);
        }
        
        public async Task Delete(IamRole role)
        {
            var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(role.Delete());
        
            if(delete.ElementId == role.ElementId)
                _logger.LogInformation("Node with elementId {ElementId} was deleted successfully", role.ElementId);
            else
                _logger.LogWarning("Node with elementId {ElementId} was not deleted, or was not found for deletion", role.ElementId);
        }
        
        public async Task<string> Update(IamRole role)
        {
            var queryList = new List<string?>
            {
                role.UpdateName(),
                role.UpdatePermissions(),
                role.UpdateModifiedRelationship(),
                role.UpdateModifiedOnRelationship()
            };

            return await _neo4JDataAccess.RunTransaction(queryList);
        }
    }
}