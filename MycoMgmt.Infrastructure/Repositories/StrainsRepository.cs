using Microsoft.Extensions.Logging;
using MycoMgmt.Domain.Models;
using MycoMgmt.Infrastructure.DataStores.Neo4J;
using Neo4j.Driver;
using Newtonsoft.Json;

namespace MycoMgmt.Infrastructure.Repositories
{
    public class StrainsRepository : BaseRepository<Strain>
    {
        private readonly INeo4JDataAccess _neo4JDataAccess;
        private ILogger<StrainsRepository> _logger;

        public StrainsRepository(INeo4JDataAccess neo4JDataAccess, ILogger<StrainsRepository> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }
        
        public override async Task<string> SearchByName(Strain strain)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(strain.SearchByNameQuery(), "x");
            return JsonConvert.SerializeObject(result);
        }

        public override async Task<string> GetByName(Strain strain)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(strain.GetByNameQuery(), "x");

            return JsonConvert.SerializeObject(result);
        }

        public override async Task<string> GetById(Strain strain)
        {
            var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(strain.GetByIdQuery());
            return JsonConvert.SerializeObject(result);
        }
    
        public override async Task<string> GetAll(Strain strain, int skip, int limit)
        {
            var result = await _neo4JDataAccess.ExecuteReadListAsync(strain.GetAllQuery(skip, limit), "result");
            return JsonConvert.SerializeObject(result);
        }
        
        public override async Task<List<IEntity>> Create(Strain strain)
        {
            var queryList = strain.CreateQueryList();
            return await _neo4JDataAccess.RunTransaction(queryList);
        }
        
        public override async Task<string> Update(Strain strain)
        {
            var queryList = strain.UpdateQueryList();
            var results = await _neo4JDataAccess.RunTransaction(queryList);
            return JsonConvert.SerializeObject(results);
        }
        
        public override async Task Delete(Strain strain)
        {
            var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(strain.Delete());
        
            if(delete.Id.ToString() == strain.Id)
                _logger.LogInformation("Node with Id {Id} was deleted successfully", strain.Id);
            else
                _logger.LogWarning("Node with Id {Id} was not deleted, or was not found for deletion", strain.Id);
        }
    }
}