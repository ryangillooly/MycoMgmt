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

        return await PersistToDatabase(spawn);
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
            // Updated ModifiedOn Relationship
            $@"
                MATCH 
                    (s:Spawn)
                WHERE
                    elementId(s) = '{elementId}'
                OPTIONAL MATCH
                    (s)-[r:MODIFIED_ON]->(d)
                DELETE 
                    r
                WITH
                    s
                MATCH
                    (d:Day {{ day: {parsedDateTime.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {parsedDateTime.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {parsedDateTime.Year} }})
                MERGE
                    (s)-[r:MODIFIED_ON]->(d)
                RETURN 
                    r
            ",
            
            // Update Modified Relationship
            $@"
                MATCH 
                    (s:Spawn)
                WHERE
                    elementId(s) = '{elementId}'
                OPTIONAL MATCH
                    (u)-[r:MODIFIED]->(s)
                DELETE
                    r
                WITH
                    s
                MATCH
                    (u:User {{ Name: '{spawn.ModifiedBy}'}} )
                MERGE 
                    (u)-[r:MODIFIED]->(s)
                RETURN
                    r            
            "
        };
        
        // Update Name
        if (!string.IsNullOrEmpty(spawn.Name))
            queryList.Add(query + $"SET s.Name = '{spawn.Name}' RETURN s");
        
        // Update Type
        if (!string.IsNullOrEmpty(spawn.Type))
            queryList.Add(query + $"SET s.Type = '{spawn.Type}' RETURN s");
        
        // Update Recipe
        // TODO
        
        // Update Location
        if (!string.IsNullOrEmpty(spawn.Location))
        {
            queryList.Add($@"
                MATCH 
                    (s:Spawn)
                WHERE
                    elementId(s) = '{elementId}'
                OPTIONAL MATCH
                    (s)-[r:STORED_IN]->(:Location)
                DELETE 
                    r
                WITH
                    s
                MATCH
                    (l:Location {{ Name: '{spawn.Location}' }})
                MERGE
                    (s)-[r:STORED_IN]->(l) 
                RETURN 
                    r
            ");
        }
        
        // Update Parent
        if (spawn.Parent != null)
        {
            queryList.Add($@"
                MATCH 
                    (s:Spawn)
                WHERE
                    elementId(s) = '{elementId}'
                OPTIONAL MATCH
                    (s)-[r:HAS_PARENT]->(n)
                DELETE
                    r
                WITH
                    s
                MATCH 
                    (p {{Name: '{spawn.Parent}' }})
                MERGE 
                    (s)-[r:HAS_PARENT]->(p) 
                RETURN 
                    r
            ");
        }
        
        // Update Child
        if (spawn.Child != null)
        {
            queryList.Add($@"
                MATCH 
                    (s:Spawn)
                WHERE
                    elementId(s) = '{elementId}'
                OPTIONAL MATCH
                    (s)<-[r:HAS_PARENT]-(n)
                DELETE
                    r
                WITH
                    s
                MATCH 
                    (p {{Name: '{spawn.Child}' }})
                MERGE 
                    (s)<-[r:HAS_PARENT]-(p) 
                RETURN 
                    r
            ");
        }
        
        // Update Successful + Finished
        if (spawn.Finished == true || spawn.Successful != null)
        {
            queryList.Add(
            // Create IsSuccessful Label on Spawn
        $@"
                MATCH 
                    (s:Spawn)
                WHERE 
                    elementId(s) = '{elementId}'
                REMOVE 
                    s :InProgress:Successful:Failed
                WITH 
                    s                    
                SET 
                    s {spawn.IsSuccessful()}
                RETURN 
                    s
            ");
        }
        else
        {
            queryList.Add(
            // Create InProgress Label on Spawn
        $@"
                MATCH 
                    (s:Spawn)
                WHERE 
                    elementId(s) = '{elementId}'
                REMOVE 
                    s :InProgress:Successful:Failed
                WITH 
                    s                    
                SET 
                    s :InProgress
                RETURN 
                    s
            ");
        }
        
        var spawnData = await _neo4JDataAccess.RunTransaction(queryList);
        return JsonConvert.SerializeObject(spawnData, Formatting.Indented);
    }
    
    private async Task<string> PersistToDatabase(Spawn spawn)
    {
        try
        {
            var queryList = CreateQueryList(spawn);
            var result = await _neo4JDataAccess.RunTransaction(queryList);
            return result;
        }
        catch (ClientException ex)
        {
            if (!Regex.IsMatch(ex.Message, @"Node\(\d+\) already exists with *"))
                throw;

            return JsonConvert.SerializeObject(new { Message = $"A spawn already exists with the name {spawn.Name}" });
        }
        catch (Exception ex)
        {
            throw new ArgumentException(ex.Message);
        }
    }

    private static List<string> CreateQueryList(Spawn spawn)
    {
        var queryList = new List<string>
        {
            /*
            // Create New Spawn
            $@"
                CREATE (s:Spawn {{
                                    Name:  '{spawn.Name}',
                                    Type:  '{spawn.Type}'
                                  }}) 
                RETURN s;
            ",
            // Create Relationship Between Spawn and Location
            $@"
                MATCH 
                    (s:Spawn  {{ Name: '{spawn.Name}' }}), 
                    (l:Location {{ Name: '{spawn.Location}' }})
                MERGE
                    (s)-[r:STORED_IN]->(l)
                RETURN r
            ",
            */
            spawn.Create(),
            spawn.CreateLocationRelationship(),
            spawn.CreateCreatedOnRelationship(),
            spawn.CreateCreatedRelationship()
        };
        
        /*
        // Create Relationship Between Spawn and Day
        queryList.Add(spawn.CreateCreatedOnRelationship());
        
        // Create Relationship Between Spawn and User 
        queryList.Add(spawn.CreateCreatedRelationship());
        */
        
        if (spawn.Parent != null)
        {
            queryList.Add(spawn.CreateParentRelationship());
        }

        queryList.Add(spawn.CreateNodeLabels());
        
        return queryList;
    }
}