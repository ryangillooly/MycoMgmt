using Microsoft.Extensions.Logging;using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.Infrastructure.DataStores.Neo4J;
using Neo4j.Driver;
using Newtonsoft.Json;
#pragma warning disable CS8604

// ReSharper disable once CheckNamespace
namespace MycoMgmt.Infrastructure.Repositories
{
    public class RoleRepository
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
    
        public async Task<string> GetAll(IamRole role, int skip, int limit)
        {
            var result = await _neo4JDataAccess.ExecuteReadListAsync(role.GetAllQuery(skip, limit), "x");
            return JsonConvert.SerializeObject(result);
        }
        
        public async Task<List<IEntity>> Create(IamRole role)
        {
            var queryList = role.CreateQueryList();      
            return await _neo4JDataAccess.RunTransaction(queryList);
        }
        
        public async Task Delete(IamRole role)
        {
            var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(role.Delete());
        
            if(delete.Id.ToString() == role.Id)
                _logger.LogInformation("Node with Id {Id} was deleted successfully", role.Id);
            else
                _logger.LogWarning("Node with Id {Id} was not deleted, or was not found for deletion", role.Id);
        }
        
        public async Task<string> Update(IamRole role)
        {
            var queryList = role.UpdateQueryList();      
            var results = await _neo4JDataAccess.RunTransaction(queryList);
            return JsonConvert.SerializeObject(results);
        }
    }
}