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

public class SpawnRepository : ISpawnRepository
{
    private readonly INeo4JDataAccess _neo4JDataAccess;
    private ILogger<SpawnRepository> _logger;
        
    public SpawnRepository(INeo4JDataAccess neo4JDataAccess, ILogger<SpawnRepository> logger)
    {
        _neo4JDataAccess = neo4JDataAccess;
        _logger = logger;
    }

    public async Task<string> SearchByName(string name)
    {
        var query = $"MATCH (s:Spawn) WHERE toUpper(s.Name) CONTAINS toUpper('{ name }') RETURN s {{ Name: s.Name, Type: s.Type }} ORDER BY s.Name LIMIT 5";
        var spawn = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "c");

        return JsonConvert.SerializeObject(spawn);
    }

    public async Task<string> GetByName(string name)
    {
        var query = $"MATCH (s:Spawn) WHERE toUpper(s.Name) = toUpper('{ name }') RETURN s {{ Name: s.Name, Type: s.Type }} ORDER BY s.Name LIMIT 5";
        var spawn = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "s");

        return JsonConvert.SerializeObject(spawn);
    }

    public async Task<string> GetById(string id)
    {
        var query = $"MATCH (s:Spawn) WHERE elementId(s) = '{id}' RETURN s";

        try
        {
            var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(query);
            return JsonConvert.SerializeObject(result);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message != "The result is empty.") 
                throw;
                
            return JsonConvert.SerializeObject(new { Message = $"No results were found for Spawn Id { id }" });
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
    
    public async Task<string> GetAll()
    {
        const string query = "MATCH (s:Spawn) RETURN s ORDER BY s.Name ASC";
        var spawn = await _neo4JDataAccess.ExecuteReadListAsync(query, "s");
        return JsonConvert.SerializeObject(spawn);
    }

    public async Task<long> GetCount()
    {
        const string query = "Match (s:Spawn) RETURN count(s) as SpawnCount";
        var count = await _neo4JDataAccess.ExecuteReadScalarAsync<long>(query);
        return count;
    }
    
    public async Task<string> Create(Spawn spawn)
    {
        if (spawn == null || string.IsNullOrWhiteSpace(spawn.Name))
            throw new ArgumentNullException(nameof(spawn), "Spawn must not be null");

        var queryList = new List<string>
        {
            spawn.Create(),
            spawn.CreateStrainRelationship(),
            spawn.CreateLocationRelationship(),
            spawn.CreateCreatedRelationship(),
            spawn.CreateCreatedOnRelationship(),
            spawn.CreateParentRelationship(),
            spawn.CreateChildRelationship(),
            spawn.CreateNodeLabels()
        };

        return await _neo4JDataAccess.RunTransaction(queryList);
    }
    
    public async Task Delete(string elementId)
    {
        var query = $"MATCH (s:Spawn) WHERE elementId(s) = '{ elementId }' DETACH DELETE s RETURN s";
        var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(query);

        if(delete.ElementId != elementId)
            _logger.LogWarning("Node with elementId {ElementId} was not deleted, or was not found for deletion", elementId);
        
        _logger.LogInformation("Node with elementId {ElementId} was deleted successfully", elementId);
    }
        
    public async Task<string> Update(string elementId, Spawn spawn)
    {
        var query = $"MATCH (s:Spawn) WHERE elementId(s) = '{elementId}' ";

        DateTime.TryParse(spawn.ModifiedOn.ToString(), out var parsedDateTime);
        
        var queryList = new List<string>
        {
            spawn.UpdateModifiedOnRelationship(elementId),
            spawn.UpdateModifiedRelationship(elementId),
            spawn.UpdateStatus(elementId)
        };
        
        // Update Name
        if (!string.IsNullOrEmpty(spawn.Name))
            queryList.Add(query + $"SET s.Name = '{spawn.Name}' RETURN s");
        
        // Update Type
        if (!string.IsNullOrEmpty(spawn.Type))
            queryList.Add(query + $"SET s.Type = '{spawn.Type}' RETURN s");
        
        // Update Notes
        if(!string.IsNullOrEmpty(spawn.Notes))
            queryList.Add(query + $"SET s.Notes = '{spawn.Notes}' RETURN s");
        
        // Update Recipe
        if (!string.IsNullOrEmpty(spawn.Recipe))
            queryList.Add(spawn.UpdateRecipeRelationship(elementId));
        
        // Update Location
        if (!string.IsNullOrEmpty(spawn.Location))
            queryList.Add(spawn.UpdateLocationRelationship(elementId));
        
        // Update Parent
        if (spawn.Parent != null)
            queryList.Add(spawn.UpdateParentRelationship(elementId));
        
        // Update Child
        if (spawn.Child != null)
            queryList.Add(spawn.UpdateChildRelationship(elementId));
        
        var spawnData = await _neo4JDataAccess.RunTransaction(queryList);
        return JsonConvert.SerializeObject(spawnData, Formatting.Indented);
    }
}