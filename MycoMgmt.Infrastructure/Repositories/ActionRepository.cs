using Microsoft.Extensions.Logging;
using MycoMgmt.Domain.Models;
using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.Infrastructure.DataStores.Neo4J;
using Neo4j.Driver;
using Newtonsoft.Json;
#pragma warning disable CS8604

// ReSharper disable once CheckNamespace
namespace MycoMgmt.Infrastructure.Repositories
{
    public class ActionRepository : BaseRepository
    {
        private readonly INeo4JDataAccess _neo4JDataAccess;
        private readonly ILogger<ActionRepository> _logger;
        
        public override async Task<List<IEntity>> Create(ModelBase model)
        {
            var queryList = model.CreateQueryList();
            return await _neo4JDataAccess.RunTransaction(queryList);
        }
        
        public override async Task<string> Update(ModelBase model)
        {
            var queryList = model.UpdateQueryList();
            var results = await _neo4JDataAccess.RunTransaction(queryList);
            return JsonConvert.SerializeObject(results);
        }
        
        public override async Task Delete(ModelBase model)
        {
            var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(model.Delete());
        
            if(delete.Id.ToString() == model.Id)
                _logger.LogInformation("Node with Id {Id} was deleted successfully", model.Id);
            else
                _logger.LogWarning("Node with Id {Id} was not deleted, or was not found for deletion", model.Id);
        }
        
        public override async Task<string> SearchByName(ModelBase model)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(model.SearchByNameQuery(), "x");
            return JsonConvert.SerializeObject(result);
        }

        public override async Task<string> GetByName(ModelBase model)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(model.GetByNameQuery(), "x");
            return JsonConvert.SerializeObject(result);
        }

        public override async Task<string> GetById(ModelBase model)
        {
            var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(model.GetByIdQuery());
            return JsonConvert.SerializeObject(result);
        }
    
        public override async Task<string> GetAll(ModelBase model, int skip, int limit)
        {
            var result = await _neo4JDataAccess.ExecuteReadListAsync(model.GetAllQuery(skip, limit), "x");
            return JsonConvert.SerializeObject(result);
        }
    }
}