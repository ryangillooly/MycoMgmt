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

public class FruitRepository : IFruitRepository
{
    private readonly INeo4JDataAccess _neo4JDataAccess;
    private ILogger<FruitRepository> _logger;
        
    public FruitRepository(INeo4JDataAccess neo4JDataAccess, ILogger<FruitRepository> logger)
    {
        _neo4JDataAccess = neo4JDataAccess;
        _logger = logger;
    }

    
    public async Task<string> Create(Fruit fruit)
    {
        var queryList = new List<string?>
        {
            fruit.Create(),
            fruit.CreateInoculatedRelationship(),
            fruit.CreateInoculatedOnRelationship(),
            fruit.CreateFinishedOnRelationship(),
            fruit.CreateStrainRelationship(),
            fruit.CreateLocationRelationship(),
            fruit.CreateCreatedRelationship(),
            fruit.CreateCreatedOnRelationship(),
            fruit.CreateParentRelationship(),
            fruit.CreateChildRelationship(),
            fruit.CreateNodeLabels()
        };

        return await _neo4JDataAccess.RunTransaction(queryList);
    }
    
    public async Task Delete(Fruit fruit)
    {
        var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(fruit.Delete());
        
        if(delete.ElementId == fruit.ElementId)
            _logger.LogInformation("Node with elementId {ElementId} was deleted successfully", fruit.ElementId);
        else
            _logger.LogWarning("Node with elementId {ElementId} was not deleted, or was not found for deletion", fruit.ElementId);
    }
        
    public async Task<string> Update(Fruit fruit)
    {        
        var queryList = new List<string?>
        {
            fruit.UpdateName(),
            fruit.UpdateInoculatedRelationship(),
            fruit.UpdateInoculatedOnRelationship(),
            fruit.UpdateFinishedOnRelationship(),
            fruit.UpdateRecipeRelationship(),
            fruit.UpdateLocationRelationship(),
            fruit.UpdateParentRelationship(),
            fruit.UpdateChildRelationship(),
            fruit.UpdateModifiedOnRelationship(),
            fruit.UpdateModifiedRelationship(),
            fruit.UpdateStatus()
        };

        var results = await _neo4JDataAccess.RunTransaction(queryList);
        return JsonConvert.SerializeObject(results);
    }
    
    public async Task<string> SearchByName(Fruit fruit)
    {
        var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(fruit.SearchByNameQuery(), "x");
        return JsonConvert.SerializeObject(result);
    }

    public async Task<string> GetByName(Fruit fruit)
    {
        var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(fruit.GetByNameQuery(), "x");

        return JsonConvert.SerializeObject(result);
    }

    public async Task<string> GetById(Fruit fruit)
    {
        var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(fruit.GetByIdQuery());
        return JsonConvert.SerializeObject(result);
    }
    
    public async Task<string> GetAll(Fruit fruit, int? skip, int? limit)
    {
        skip  = skip ?? 0;
        limit = limit ?? 0;
        
        var result = await _neo4JDataAccess.ExecuteReadListAsync(fruit.GetAll(skip, limit), "x");
        return JsonConvert.SerializeObject(result);
    }
}