using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using MycoMgmt.API.Helpers;
using MycoMgmt.API.DataStores.Neo4J;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.Helpers;
using Neo4j.Driver;
using Newtonsoft.Json;
#pragma warning disable CS8604

// ReSharper disable once CheckNamespace
namespace MycoMgmt.API.Repositories;

public class BulkRepository : IBulkRepository
{
    private readonly INeo4JDataAccess _neo4JDataAccess;
    private ILogger<BulkRepository> _logger;
        
    public BulkRepository(INeo4JDataAccess neo4JDataAccess, ILogger<BulkRepository> logger)
    {
        _neo4JDataAccess = neo4JDataAccess;
        _logger = logger;
    }

    public async Task<string> SearchByName(Bulk bulk)
    {
        var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(bulk.SearchByNameQuery(), "x");
        return JsonConvert.SerializeObject(result);
    }

    public async Task<string> GetByName(Bulk bulk)
    {
        var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(bulk.GetByNameQuery(), "x");

        return JsonConvert.SerializeObject(result);
    }

    public async Task<string> GetById(Bulk bulk)
    {
        var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(bulk.GetByIdQuery());
        return JsonConvert.SerializeObject(result);
    }
    
    public async Task<string> GetAll(Bulk bulk)
    {
        var result = await _neo4JDataAccess.ExecuteReadListAsync(bulk.GetAll(), "x");
        return JsonConvert.SerializeObject(result);
    }
    
    public async Task<string> Create(Bulk bulk)
    {
        var queryList = new List<string?>
        {
            bulk.Create(),
            bulk.CreateStrainRelationship(),
            bulk.CreateLocationRelationship(),
            bulk.CreateCreatedRelationship(),
            bulk.CreateCreatedOnRelationship(),
            bulk.CreateRecipeRelationship(),
            bulk.CreateParentRelationship(),
            bulk.CreateChildRelationship(),
            bulk.CreateNodeLabels()
        };

        return await _neo4JDataAccess.RunTransaction(queryList);
    }
    
    public async Task Delete(Bulk bulk)
    {
        var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(bulk.Delete());

        if(delete.ElementId == bulk.ElementId)
            _logger.LogInformation("Node with elementId {ElementId} was deleted successfully", bulk.ElementId);
        else    
            _logger.LogWarning("Node with elementId {ElementId} was not deleted, or was not found for deletion", bulk.ElementId);
        
    }
        
    public async Task<string> Update(Bulk bulk)
    {
        var queryList = new List<string?>
        {
            bulk.UpdateName(),
            bulk.UpdateNotes(),
            bulk.UpdateRecipeRelationship(),
            bulk.UpdateLocationRelationship(),
            bulk.UpdateParentRelationship(),
            bulk.UpdateChildRelationship(),
            bulk.UpdateModifiedOnRelationship(),
            bulk.UpdateModifiedRelationship(),
            bulk.UpdateStatus()
        };

        var results = await _neo4JDataAccess.RunTransaction(queryList);
        return JsonConvert.SerializeObject(results);
    }
}