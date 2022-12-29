using MycoMgmt.API.Helpers;
using MycoMgmt.API.DataStores.Neo4J;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.Helpers;
using Neo4j.Driver;
using Newtonsoft.Json;
#pragma warning disable CS8604

// ReSharper disable once CheckNamespace
namespace MycoMgmt.API.Repositories;

public class CultureRepository : ICultureRepository
{
    private readonly INeo4JDataAccess _neo4JDataAccess;
    private ILogger<CultureRepository> _logger;
        
    public CultureRepository(INeo4JDataAccess neo4JDataAccess, ILogger<CultureRepository> logger)
    {
        _neo4JDataAccess = neo4JDataAccess;
        _logger = logger;
    }

    public async Task<string> Create(Culture culture)
    {
        var queryList = new List<string?>
        {
            culture.Create(),
            culture.CreateInoculatedRelationship(),
            culture.CreateInoculatedOnRelationship(),
            culture.CreateFinishedOnRelationship(),
            culture.CreateStrainRelationship(),
            culture.CreateLocationRelationship(),
            culture.CreateCreatedRelationship(),
            culture.CreateCreatedOnRelationship(),
            culture.CreateParentRelationship(),
            culture.CreateRecipeRelationship(),
            culture.CreateChildRelationship(),
            culture.CreateNodeLabels(),
            culture.CreateVendorRelationship()
        };

        return await _neo4JDataAccess.RunTransaction(queryList);
    }
    
    public async Task Delete(Culture culture)
    {
        var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(culture.Delete());
        
        if(delete.ElementId == culture.ElementId)
            _logger.LogInformation("Node with elementId {ElementId} was deleted successfully", culture.ElementId);
        else
            _logger.LogWarning("Node with elementId {ElementId} was not deleted, or was not found for deletion", culture.ElementId);
    }
        
    public async Task<string> Update(Culture culture)
    {
        var queryList = new List<string?>
        {
            culture.UpdateName(),
            culture.UpdateNotes(),
            culture.UpdateType(),
            culture.UpdateInoculatedRelationship(),
            culture.UpdateInoculatedOnRelationship(),
            culture.UpdateFinishedOnRelationship(),
            culture.UpdateRecipeRelationship(),
            culture.UpdateStrainRelationship(),
            culture.UpdateLocationRelationship(),
            culture.UpdateParentRelationship(),
            culture.UpdateChildRelationship(),
            culture.UpdateVendorRelationship(),
            culture.UpdateModifiedOnRelationship(),
            culture.UpdateModifiedRelationship(),
            culture.UpdateStatus(),
            culture.UpdateStatusLabel(),
        };
        
        var results = await _neo4JDataAccess.RunTransaction(queryList);
        return JsonConvert.SerializeObject(results);
    }
    
    public async Task<string> SearchByName(Culture culture)
    {
        var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(culture.SearchByNameQuery(), "x");
        return JsonConvert.SerializeObject(result);
    }

    public async Task<string> GetByName(Culture culture)
    {
        var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(culture.GetByNameQuery(), "x");

        return JsonConvert.SerializeObject(result);
    }

    public async Task<string> GetById(Culture culture)
    {
        var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(culture.GetByIdQuery());
        return JsonConvert.SerializeObject(result);
    }
    
    public async Task<string> GetAll(Culture culture, int? skip, int? limit)
    {
        skip  = skip ?? 0;
        limit = limit ?? 10;
        
        var result = await _neo4JDataAccess.ExecuteReadListAsync(culture.GetAll(skip, limit), "result");
        return JsonConvert.SerializeObject(result);
    }
}