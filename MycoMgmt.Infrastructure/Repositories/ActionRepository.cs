using Microsoft.Extensions.Logging;
using MycoMgmt.Domain.Models;
using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.Infrastructure.DataStores.Neo4J;
using Neo4j.Driver;
using Newtonsoft.Json;
using ILogger = Neo4j.Driver.ILogger;

#pragma warning disable CS8604

// ReSharper disable once CheckNamespace
namespace MycoMgmt.Infrastructure.Repositories
{
    public class ActionRepository : IActionRepository
    {
        private readonly INeo4JDataAccess _neo4JDataAccess;
        private readonly ILogger<ActionRepository> _logger;

        public ActionRepository(INeo4JDataAccess dataAccess, ILogger<ActionRepository> logger)
        {
            _neo4JDataAccess = dataAccess;
            _logger = logger;
        }
        
        public async Task<List<IEntity>> Create(ModelBase model)
        {
            var queryList = model.CreateQueryList();
            return await _neo4JDataAccess.RunTransaction(queryList);
        }
        
        public async Task<List<IEntity>> Update (ModelBase model)
        {
            var queryList = model.UpdateQueryList();
            return await _neo4JDataAccess.RunTransaction(queryList);
        }
        
        public async Task Delete(ModelBase model)
        {
            var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(model.Delete());
        
            // Need to check this logic is still valid after moving away from ElementId
            if(delete.Id.ToString() == model.Id.ToString())
                _logger.LogInformation("Node with Id {Id} was deleted successfully", model.Id);
            else
                _logger.LogWarning("Node with Id {Id} was not deleted, or was not found for deletion", model.Id);
        }
        
        public async Task<string> SearchByName(ModelBase model)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(model.SearchByNameQuery(), "x");
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> GetByName(ModelBase model)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(model.GetByNameQuery(), "x");
            return JsonConvert.SerializeObject(result);
        }

        public async Task<INode> GetById(ModelBase model) => await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(model.GetByIdQuery());
    
        public async Task<string> GetAll(ModelBase model, int skip, int limit)
        {
            var result = await _neo4JDataAccess.ExecuteReadListAsync(model.GetAllQuery(skip, limit), "result");
            return JsonConvert.SerializeObject(result);
        }
    }
}