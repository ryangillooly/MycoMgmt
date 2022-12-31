using MycoMgmt.API.Helpers;
using MycoMgmt.API.DataStores.Neo4J;
using MycoMgmt.Domain.Models.Mushrooms;
using Neo4j.Driver;
using Newtonsoft.Json;
#pragma warning disable CS8604

// ReSharper disable once CheckNamespace
namespace MycoMgmt.API.Repositories;

public class SpawnRepository : ISpawnRepository
{
    private readonly INeo4JDataAccess _neo4JDataAccess;
    private ILogger<SpawnRepository> _logger;
        
    public SpawnRepository(INeo4JDataAccess neo4JDataAccess, ILogger<SpawnRepository> logger)
    {
        _neo4JDataAccess = neo4JDataAccess;
        _logger = logger;
    }
    
    public async Task<List<IEntity>> Create(Spawn spawn)
    {
        var queryList = spawn.CreateQueryList();
        return await _neo4JDataAccess.RunTransaction2(queryList);
    }
    
    public async Task<string> Update(Spawn spawn)
    {
        var queryList = spawn.UpdateQueryList();
        var spawnData = await _neo4JDataAccess.RunTransaction2(queryList);
        return JsonConvert.SerializeObject(spawnData);
    }
    
    public async Task Delete(Spawn spawn)
    {
        var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(spawn.Delete());

        if(delete.ElementId == spawn.ElementId)
            _logger.LogInformation("Node with elementId {ElementId} was deleted successfully", spawn.ElementId);
        else
            _logger.LogWarning("Node with elementId {ElementId} was not deleted, or was not found for deletion", spawn.ElementId);
    }

    public async Task<string> SearchByName(Spawn spawn)
    {
        var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(spawn.SearchByNameQuery(), "x");
        return JsonConvert.SerializeObject(result);
    }

    public async Task<string> GetByName(Spawn spawn)
    {
        var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(spawn.GetByNameQuery(), "x");
        return JsonConvert.SerializeObject(result);
    }

    public async Task<string> GetById(Spawn spawn)
    {
        var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(spawn.GetByIdQuery());
        return JsonConvert.SerializeObject(result);
    }
    
    public async Task<string> GetAll(Spawn spawn, int skip, int limit)
    {
        var result = await _neo4JDataAccess.ExecuteReadListAsync(spawn.GetAllQuery(skip, limit), "result");
        return JsonConvert.SerializeObject(result);
    }
    
}