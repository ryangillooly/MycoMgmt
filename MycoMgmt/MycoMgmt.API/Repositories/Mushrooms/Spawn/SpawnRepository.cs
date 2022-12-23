using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using MycoMgmt.API.Helpers;
using MycoMgmt.API.DataStores.Neo4J;
using MycoMgmt.Domain.Models.Mushrooms;
using Neo4j.Driver;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace MycoMgmt.API.Repositories;

public class SpawnRepository : ISpawnRepository
{
    private readonly INeo4JDataAccess _neo4JDataAccess;
    private ILogger<CultureRepository> _logger;
        
    public SpawnRepository(INeo4JDataAccess neo4JDataAccess, ILogger<CultureRepository> logger)
    {
        _neo4JDataAccess = neo4JDataAccess;
        _logger = logger;
    }

    public async Task<string> SearchByName(string name)
    {
        var query = $"MATCH (c:Culture) WHERE toUpper(c.Name) CONTAINS toUpper('{ name }') RETURN c{{ Name: c.Name, Type: c.Type }} ORDER BY c.Name LIMIT 5";
        var cultures = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "c");

        return JsonConvert.SerializeObject(cultures);
    }

    public async Task<string> GetByName(string name)
    {
        var query = $"MATCH (c:Culture) WHERE toUpper(c.Name) = toUpper('{ name }') RETURN c{{ Name: c.Name, Type: c.Type }} ORDER BY c.Name LIMIT 5";
        var cultures = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "c");

        return JsonConvert.SerializeObject(cultures);
    }

    public async Task<string> GetById(string id)
    {
        var query = $"MATCH (c:Culture) WHERE elementId(c) = '{id}' RETURN c";

        try
        {
            var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(query);
            return JsonConvert.SerializeObject(result);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message != "The result is empty.") 
                throw;
                
            return JsonConvert.SerializeObject(new { Message = $"No results were found for Culture Id { id }" });
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
    
    public async Task<string> GetAll()
    {
        const string query = "MATCH (c:Culture) RETURN c ORDER BY c.Name ASC";
        var cultures = await _neo4JDataAccess.ExecuteReadListAsync(query, "c");
        return JsonConvert.SerializeObject(cultures);
    }

    public async Task<long> GetCount()
    {
        const string query = "Match (c:Culture) RETURN count(c) as CultureCount";
        var count = await _neo4JDataAccess.ExecuteReadScalarAsync<long>(query);
        return count;
    }
    
    public async Task<string> Create(Spawn spawn)
    {
        if (spawn == null || string.IsNullOrWhiteSpace(spawn.Name))
            throw new ArgumentNullException(nameof(spawn), "Culture must not be null");

        return await PersistToDatabase(spawn);
    }
    
    public async Task Delete(string elementId)
    {
        var query = $"MATCH (c:Culture) WHERE elementId(c) = '{ elementId }' DETACH DELETE c RETURN c";
        var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(query);

        if(delete.ElementId != elementId)
            _logger.LogWarning("Node with elementId {ElementId} was not deleted, or was not found for deletion", elementId);
        
        _logger.LogInformation("Node with elementId {ElementId} was deleted successfully", elementId);
    }
        
    public async Task<string> Update(string elementId, Spawn spawn)
    {
        var query = $"MATCH (c:Culture) WHERE elementId(c) = '{elementId}' ";

        DateTime.TryParse(spawn.ModifiedOn.ToString(), out var parsedDateTime);
        
        var queryList = new List<string>
        {
            // Updated ModifiedOn Relationship
            $@"
                MATCH 
                    (c:Culture)
                WHERE
                    elementId(c) = '{elementId}'
                OPTIONAL MATCH
                    (c)-[r:MODIFIED_ON]->(d)
                DELETE 
                    r
                WITH
                    c
                MATCH
                    (d:Day {{ day: {parsedDateTime.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {parsedDateTime.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {parsedDateTime.Year} }})
                MERGE
                    (c)-[r:MODIFIED_ON]->(d)
                RETURN 
                    r
            ",
            
            // Update Modified Relationship
            $@"
                MATCH 
                    (c:Culture)
                WHERE
                    elementId(c) = '{elementId}'
                OPTIONAL MATCH
                    (u)-[r:MODIFIED]->(c)
                DELETE
                    r
                WITH
                    c
                MATCH
                    (u:User {{ Name: '{spawn.ModifiedBy}'}} )
                MERGE 
                    (u)-[r:MODIFIED]->(c)
                RETURN
                    r            
            "
        };
        
        // Update Name
        if (!string.IsNullOrEmpty(spawn.Name))
            queryList.Add(query + $"SET c.Name = '{spawn.Name}' RETURN c");
        
        // Update Type
        if (!string.IsNullOrEmpty(spawn.Type))
            queryList.Add(query + $"SET c.Type = '{spawn.Type}' RETURN c");
        
        // Update Recipe
        // TODO
        
        // Update Location
        if (!string.IsNullOrEmpty(spawn.Location))
        {
            queryList.Add($@"
                MATCH 
                    (c:Culture)
                WHERE
                    elementId(c) = '{elementId}'
                OPTIONAL MATCH
                    (c)-[r:STORED_IN]->(:Location)
                DELETE 
                    r
                WITH
                    c
                MATCH
                    (l:Location {{ Name: '{spawn.Location}' }})
                MERGE
                    (c)-[r:STORED_IN]->(l) 
                RETURN 
                    r
            ");
        }
        
        // Update Parent
        if (spawn.Parent != null)
        {
            queryList.Add($@"
                MATCH 
                    (c:Culture)
                WHERE
                    elementId(c) = '{elementId}'
                OPTIONAL MATCH
                    (c)-[r:HAS_PARENT]->(n)
                DELETE
                    r
                WITH
                    c
                MATCH 
                    (p {{Name: '{spawn.Parent}' }})
                MERGE 
                    (c)-[r:HAS_PARENT]->(p) 
                RETURN 
                    r
            ");
        }
        
        // Update Child
        if (spawn.Child != null)
        {
            queryList.Add($@"
                MATCH 
                    (c:Culture)
                WHERE
                    elementId(c) = '{elementId}'
                OPTIONAL MATCH
                    (c)<-[r:HAS_PARENT]-(n)
                DELETE
                    r
                WITH
                    c
                MATCH 
                    (p {{Name: '{spawn.Child}' }})
                MERGE 
                    (c)<-[r:HAS_PARENT]-(p) 
                RETURN 
                    r
            ");
        }
        
        // Update Successful + Finished
        if (spawn.Finished == true || spawn.Successful != null)
        {
            queryList.Add(
            // Create IsSuccessful Label on Culture
        $@"
                MATCH 
                    (c:Culture)
                WHERE 
                    elementId(c) = '{elementId}'
                REMOVE 
                    c :InProgress:Successful:Failed
                WITH 
                    c                    
                SET 
                    c {spawn.IsSuccessful()}
                RETURN 
                    c
            ");
        }
        else
        {
            queryList.Add(
            // Create InProgress Label on Culture
        $@"
                MATCH 
                    (c:Culture)
                WHERE 
                    elementId(c) = '{elementId}'
                REMOVE 
                    c :InProgress:Successful:Failed
                WITH 
                    c                    
                SET 
                    c :InProgress
                RETURN 
                    c
            ");
        }
        
        var cultures = await _neo4JDataAccess.RunTransaction(queryList);
        return JsonConvert.SerializeObject(cultures, Formatting.Indented);
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

            return JsonConvert.SerializeObject(new { Message = $"A culture already exists with the name {spawn.Name}" });
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
            // Create New Culture
            $@"
                CREATE (c:Culture {{
                                    Name:  '{spawn.Name}',
                                    Type:  '{spawn.Type}'
                                  }}) 
                RETURN c;
            ",
            // Create Relationship Between Culture and Location
            $@"
                MATCH 
                    (c:Culture  {{ Name: '{spawn.Name}' }}), 
                    (l:Location {{ Name: '{spawn.Location}' }})
                MERGE
                    (c)-[r:STORED_IN]->(l)
                RETURN r
            ",
            // Create Relationship Between Culture and Day
            $@"
                MATCH 
                    (c:Culture {{ Name: '{spawn.Name}' }}), 
                    (d:Day     {{ day: {spawn.CreatedOn.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {spawn.CreatedOn.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {spawn.CreatedOn.Year} }})
                MERGE
                    (c)-[r:CREATED_ON]->(d)
                RETURN r
            ",
            // Create Relationship Between Culture and User 
            $@"
                MATCH 
                    (c:Culture {{ Name: '{spawn.Name}'      }}),
                    (u:User    {{ Name: '{spawn.CreatedBy}' }})
                MERGE
                    (u)-[r:CREATED]->(c)
                RETURN r
            "
        };

        if (spawn.Parent != null)
        {
            queryList.Add(
                // Create Relationship Between Culture and Parent
                $@"
                        MATCH 
                            (c:Culture {{ Name: '{spawn.Name}' }}), 
                            (p {{ Name: '{spawn.Parent}' }})
                        MERGE
                            (c)-[r:HAS_PARENT]->(p)
                        RETURN r
                    ");
        }

        if (spawn.Finished == true)
        {
            queryList.Add(
                // Create IsSuccessful Label on Culture
                $@"
                        MATCH (c:Culture {{ Name: '{spawn.Name}' }})
                        SET c {spawn.IsSuccessful()}
                        RETURN c
                    ");
        }
        else
        {
            queryList.Add(
                // Create InProgress Label on Culture
                $@"
                        MATCH (c:Culture {{ Name: '{spawn.Name}' }})
                        SET c :InProgress
                        RETURN c
                    ");
        }
        
        return queryList;
    }
}