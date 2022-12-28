using MycoMgmt.API.Helpers;
using MycoMgmt.API.DataStores.Neo4J;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.Helpers;
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
    
    public async Task<string> Create(Spawn spawn)
    {
        var queryList = new List<string?>
        {
            spawn.Create(),
            spawn.CreateInoculatedRelationship(),
            spawn.CreateInoculatedOnRelationship(),
            spawn.CreateFinishedOnRelationship(),
            spawn.CreateStrainRelationship(),
            spawn.CreateLocationRelationship(),
            spawn.CreateCreatedRelationship(),
            spawn.CreateCreatedOnRelationship(),
            spawn.CreateParentRelationship(),
            spawn.CreateChildRelationship(),
            spawn.CreateRecipeRelationship(),
            spawn.CreateNodeLabels()
        };

        return await _neo4JDataAccess.RunTransaction(queryList);
    }
    
    public async Task<string> Update(Spawn spawn)
    {
        var queryList = new List<string?>
        {
            spawn.UpdateName(),
            spawn.UpdateNotes(),
            spawn.UpdateType(),
            spawn.UpdateInoculatedRelationship(),
            spawn.UpdateInoculatedOnRelationship(),
            spawn.UpdateFinishedOnRelationship(),
            spawn.UpdateModifiedOnRelationship(),
            spawn.UpdateModifiedRelationship(),
            spawn.UpdateRecipeRelationship(),
            spawn.UpdateLocationRelationship(),
            spawn.UpdateParentRelationship(),
            spawn.UpdateChildRelationship(),
            spawn.UpdateStatus(),
            spawn.UpdateStatusLabel(),
        };
        
        var spawnData = await _neo4JDataAccess.RunTransaction(queryList);
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
    
    public async Task<string> GetAll(Spawn spawn, int? skip, int? limit)
    {
        skip  = skip ?? 0;
        limit = limit ?? 10;
        
        var result = await _neo4JDataAccess.ExecuteReadListAsync(spawn.GetAll(skip, limit), "result");
        return JsonConvert.SerializeObject(spawn);
    }
    
}