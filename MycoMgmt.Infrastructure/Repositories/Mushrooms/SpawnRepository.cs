using Microsoft.Extensions.Logging;
using MycoMgmt.Domain.Models;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.Infrastructure.DataStores.Neo4J;
using Neo4j.Driver;
using Newtonsoft.Json;
#pragma warning disable CS8604

// ReSharper disable once CheckNamespace
namespace MycoMgmt.Infrastructure.Repositories;

public class SpawnRepository : BaseRepository<Spawn>
{
    private readonly INeo4JDataAccess _neo4JDataAccess;
    private ILogger<SpawnRepository> _logger;
        
    public SpawnRepository(INeo4JDataAccess neo4JDataAccess, ILogger<SpawnRepository> logger)
    {
        _neo4JDataAccess = neo4JDataAccess;
        _logger = logger;
    }
    
    public override async Task<List<IEntity>> Create(Spawn spawn)
    {
        var queryList = spawn.CreateQueryList();
        return await _neo4JDataAccess.RunTransaction(queryList);
    }
    
    public override async Task<string> Update(Spawn spawn)
    {
        var queryList = spawn.UpdateQueryList();
        var spawnData = await _neo4JDataAccess.RunTransaction(queryList);
        return JsonConvert.SerializeObject(spawnData);
    }
    
    public override async Task Delete(Spawn spawn)
    {
        var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(spawn.Delete());

        if(delete.Id.ToString() == spawn.Id)
            _logger.LogInformation("Node with Id {Id} was deleted successfully", spawn.Id);
        else
            _logger.LogWarning("Node with Id {Id} was not deleted, or was not found for deletion", spawn.Id);
    }

    public override async Task<string> SearchByName(Spawn spawn)
    {
        var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(spawn.SearchByNameQuery(), "x");
        return JsonConvert.SerializeObject(result);
    }

    public override async Task<string> GetByName(Spawn spawn)
    {
        var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(spawn.GetByNameQuery(), "x");
        return JsonConvert.SerializeObject(result);
    }

    public override async Task<string> GetById(Spawn spawn)
    {
        var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(spawn.GetByIdQuery());
        return JsonConvert.SerializeObject(result);
    }
    
    public override async Task<string> GetAll(Spawn spawn, int skip, int limit)
    {
        var result = await _neo4JDataAccess.ExecuteReadListAsync(spawn.GetAllQuery(skip, limit), "result");
        return JsonConvert.SerializeObject(result);
    }
    
}