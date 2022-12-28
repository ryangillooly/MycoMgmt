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
    
    public async Task<string> GetAll()
    {
        const string query = "MATCH (s:Spawn) RETURN s ORDER BY s.Name ASC";
        var spawn = await _neo4JDataAccess.ExecuteReadListAsync(query, "s");
        
        return JsonConvert.SerializeObject(spawn);
    }
    
    public async Task<string> Create(Spawn spawn)
    {
        var queryList = new List<string>
        {
            spawn.Create(),
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
    
    public async Task<string> Update(string elementId, Spawn spawn)
    {
        var queryList = new List<string>
        {
            spawn.UpdateModifiedOnRelationship(elementId),
            spawn.UpdateModifiedRelationship(elementId),
            spawn.UpdateStatus(elementId),
            spawn.UpdateName(elementId),
            spawn.UpdateNotes(elementId),
            spawn.UpdateType(elementId),
            spawn.UpdateRecipeRelationship(elementId),
            spawn.UpdateLocationRelationship(elementId),
            spawn.UpdateParentRelationship(elementId),
            spawn.UpdateChildRelationship(elementId)
        };
        
        var spawnData = await _neo4JDataAccess.RunTransaction(queryList);
        return JsonConvert.SerializeObject(spawnData);
    }
    
    public async Task Delete(string elementId)
    {
        var query = $"MATCH (s:Spawn) WHERE elementId(s) = '{ elementId }' DETACH DELETE s RETURN s";
        var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(query);

        if(delete.ElementId != elementId)
            _logger.LogWarning("Node with elementId {ElementId} was not deleted, or was not found for deletion", elementId);
        
        _logger.LogInformation("Node with elementId {ElementId} was deleted successfully", elementId);
    }
}