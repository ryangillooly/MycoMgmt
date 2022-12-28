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
    public class UserRepository : IUserRepository
    {
        private readonly INeo4JDataAccess _neo4JDataAccess;
        private ILogger<UserRepository> _logger;
        
        public UserRepository(INeo4JDataAccess neo4JDataAccess, ILogger<UserRepository> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }
        
        public async Task<string> SearchByName(User user)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(user.SearchByNameQuery(), "x");
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> GetByName(User user)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(user.GetByNameQuery(), "x");

            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> GetById(User user)
        {
            var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(user.GetByIdQuery());
            return JsonConvert.SerializeObject(result);
        }
    
        public async Task<string> GetAll(User user)
        {
            var result = await _neo4JDataAccess.ExecuteReadListAsync(user.GetAll(), "x");
            return JsonConvert.SerializeObject(result);
        }
        
        public async Task Delete(User user)
        {
            var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(user.Delete());
        
            if(delete.ElementId == user.ElementId)
                _logger.LogInformation("Node with elementId {ElementId} was deleted successfully", user.ElementId);
            else
                _logger.LogWarning("Node with elementId {ElementId} was not deleted, or was not found for deletion", user.ElementId);
        }
        
        public async Task<string> Update(User user)
        {
            var queryList = new List<string?>
            {
                user.UpdateName(),
                user.UpdateAccountRelationship(),
                user.UpdateRoleRelationship(),
                user.UpdatePermissionRelationship(),
                user.UpdateModifiedOnRelationship(),
                user.UpdateModifiedRelationship(),
            };
        
            var results = await _neo4JDataAccess.RunTransaction(queryList);
            return JsonConvert.SerializeObject(results);
        }
        
        public async Task<string> Create(User user)
        {
            var queryList = new List<string?>
            {
                user.Create(),
                user.CreateAccountRelationship(),
                user.CreateRoleRelationship(),
                user.CreatePermissionRelationship(),
                user.CreateCreatedRelationship(),
                user.CreateCreatedOnRelationship()
            };

            return await _neo4JDataAccess.RunTransaction(queryList);
        }
    }
}