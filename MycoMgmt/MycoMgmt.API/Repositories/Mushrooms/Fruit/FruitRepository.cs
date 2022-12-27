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

    public async Task<string> SearchByName(string name)
    {
        var query = $"MATCH (f:Fruit) WHERE toUpper(f.Name) CONTAINS toUpper('{ name }') RETURN f {{ Name: f.Name, Type: f.Type }} ORDER BY f.Name LIMIT 5";
        var spawn = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "c");

        return JsonConvert.SerializeObject(spawn);
    }

    public async Task<string> GetByName(string name)
    {
        var query = $"MATCH (f:Fruit) WHERE toUpper(f.Name) = toUpper('{ name }') RETURN f {{ Name: f.Name, Type: f.Type }} ORDER BY f.Name LIMIT 5";
        var spawn = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "s");

        return JsonConvert.SerializeObject(spawn);
    }

    public async Task<string> GetById(string id)
    {
        var query = $"MATCH (f:Fruit) WHERE elementId(f) = '{id}' RETURN f";

        try
        {
            var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(query);
            return JsonConvert.SerializeObject(result);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message != "The result is empty.") 
                throw;
                
            return JsonConvert.SerializeObject(new { Message = $"No results were found for Fruit Id { id }" });
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
    
    public async Task<string> GetAll()
    {
        const string query = "MATCH (f:Fruit) RETURN f ORDER BY f.Name ASC";
        var spawn = await _neo4JDataAccess.ExecuteReadListAsync(query, "f");
        return JsonConvert.SerializeObject(spawn);
    }

    public async Task<long> GetCount()
    {
        const string query = "Match (f:Fruit) RETURN count(f) as FruitCount";
        var count = await _neo4JDataAccess.ExecuteReadScalarAsync<long>(query);
        return count;
    }
    
    public async Task<string> Create(Fruit fruit)
    {
        if (fruit == null || string.IsNullOrWhiteSpace(fruit.Name))
            throw new ArgumentNullException(nameof(fruit), "Fruit must not be null");

        var queryList = new List<string>
        {
            fruit.Create(),
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
    
    public async Task Delete(string elementId)
    {
        var query = $"MATCH (f:Fruit) WHERE elementId(f) = '{ elementId }' DETACH DELETE f RETURN f";
        var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(query);

        if(delete.ElementId != elementId)
            _logger.LogWarning("Node with elementId {ElementId} was not deleted, or was not found for deletion", elementId);
        
        _logger.LogInformation("Node with elementId {ElementId} was deleted successfully", elementId);
    }
        
    public async Task<string> Update(string elementId, Fruit fruit)
    {
        var query = $"MATCH (f:Fruit) WHERE elementId(f) = '{elementId}' ";

        DateTime.TryParse(fruit.ModifiedOn.ToString(), out var parsedDateTime);
        
        var queryList = new List<string>
        {
            fruit.UpdateModifiedOnRelationship(elementId),
            fruit.UpdateModifiedRelationship(elementId),
            fruit.UpdateStatus(elementId)
        };
        
        // Update Name
        if (!string.IsNullOrEmpty(fruit.Name))
            queryList.Add(query + $"SET f.Name = '{fruit.Name}' RETURN f");
        

        /*
         USE THIS TO LOOK AT REMOVING LABELS TO CHANGE TYPE
         
         MATCH (s:Spawn)
        WHERE s.
        FOREACH (label IN labels(n) |
          REMOVE n:Successful)
        SET n:LabelToKeep
        RETURN n;
         */
        
        
        // Update Recipe
        // TODO
        
        // Update Location
        if (!string.IsNullOrEmpty(fruit.Location))
            queryList.Add(fruit.UpdateLocationRelationship(elementId));
        
        // Update Parent
        if (fruit.Parent != null)
            queryList.Add(fruit.UpdateParentRelationship(elementId));
        
        // Update Child
        if (fruit.Child != null)
            queryList.Add(fruit.UpdateChildRelationship(elementId));

        var spawnData = await _neo4JDataAccess.RunTransaction(queryList);
        return JsonConvert.SerializeObject(spawnData, Formatting.Indented);
    }
}