using Microsoft.Extensions.Logging;
using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.Infrastructure.DataStores.Neo4J;
using Neo4j.Driver;
using Newtonsoft.Json;
#pragma warning disable CS8604

// ReSharper disable once CheckNamespace
namespace MycoMgmt.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        private readonly INeo4JDataAccess _neo4JDataAccess;
        private ILogger<UserRepository> _logger;
        
        public UserRepository(INeo4JDataAccess neo4JDataAccess, ILogger<UserRepository> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }
        
        public override async Task<List<IEntity>> Create(User user)
        {
            var queryList = user.CreateQueryList();
            return await _neo4JDataAccess.RunTransaction(queryList);
        }
        
        public override async Task<string> Update(User user)
        {
            var queryList = user.UpdateQueryList();
            var results = await _neo4JDataAccess.RunTransaction(queryList);
            return JsonConvert.SerializeObject(results);
        }
        
        public override async Task Delete(User user)
        {
            var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(user.Delete());
        
            if(delete.ElementId == user.ElementId)
                _logger.LogInformation("Node with elementId {ElementId} was deleted successfully", user.ElementId);
            else
                _logger.LogWarning("Node with elementId {ElementId} was not deleted, or was not found for deletion", user.ElementId);
        }
        
        public override async Task<string> SearchByName(User user)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(user.SearchByNameQuery(), "x");
            return JsonConvert.SerializeObject(result);
        }

        public override async Task<string> GetByName(User user)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(user.GetByNameQuery(), "x");

            return JsonConvert.SerializeObject(result);
        }

        public override async Task<string> GetById(User user)
        {
            var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(user.GetByIdQuery());
            return JsonConvert.SerializeObject(result);
        }
    
        public override async Task<string> GetAll(User user, int skip, int limit)
        {
            var result = await _neo4JDataAccess.ExecuteReadListAsync(user.GetAllQuery(skip, limit), "x");
            return JsonConvert.SerializeObject(result);
        }
    }
}