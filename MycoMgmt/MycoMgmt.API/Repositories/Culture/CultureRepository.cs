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

public class CultureRepository : ICultureRepository
{
    private readonly INeo4JDataAccess _neo4JDataAccess;
    private ILogger<CultureRepository> _logger;
        
    public CultureRepository(INeo4JDataAccess neo4JDataAccess, ILogger<CultureRepository> logger)
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
        
    public async Task<string> Create(Culture culture)
    {
        if (culture == null || string.IsNullOrWhiteSpace(culture.Name))
            throw new ArgumentNullException(nameof(culture), "Culture must not be null");

        return await PersistToDatabase(culture);
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
        
    private async Task<string> PersistToDatabase(Culture culture)
    {
        try
        {
            var queryList = CreateQueryList(culture);
            var result = await _neo4JDataAccess.RunTransaction(queryList);
            return result;
        }
        catch (ClientException ex)
        {
            if (!Regex.IsMatch(ex.Message, @"Node\(\d+\) already exists with *"))
                throw;

            return JsonConvert.SerializeObject(new { Message = $"A culture already exists with the name {culture.Name}" });
        }
        catch (Exception ex)
        {
            throw new ArgumentException(ex.Message);
        }
    }

    private static List<string> CreateQueryList(Culture culture)
    {
        var queryList = new List<string>
        {
            // Create New Culture
            $@"
                CREATE (c:Culture {{
                                    Name:  '{culture.Name}',
                                    Type:  '{culture.Type}'
                                  }}) 
                RETURN c;
            ",
            // Create Relationship Between Culture and Strain
            $@"
                MATCH 
                    (c:Culture {{ Name: '{culture.Name}'   }}), 
                    (s:Strain  {{ Name: '{culture.Strain}' }})
                MERGE
                    (c)-[r:HAS_STRAIN]->(s)
                RETURN r
            ",
            // Create Relationship Between Culture and Location
            $@"
                MATCH 
                    (c:Culture  {{ Name: '{culture.Name}' }}), 
                    (l:Location {{ Name: '{culture.Location}' }})
                MERGE
                    (c)-[r:STORED_IN]->(l)
                RETURN r
            ",
            // Create Relationship Between Culture and Day
            $@"
                MATCH 
                    (c:Culture {{ Name: '{culture.Name}' }}), 
                    (d:Day     {{ day: {culture.CreatedOn.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {culture.CreatedOn.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {culture.CreatedOn.Year} }})
                MERGE
                    (c)-[r:CREATED_ON]->(d)
                RETURN r
            ",
            // Create Relationship Between Culture and User 
            $@"
                MATCH 
                    (c:Culture {{ Name: '{culture.Name}'      }}),
                    (u:User    {{ Name: '{culture.CreatedBy}' }})
                MERGE
                    (u)-[r:CREATED]->(c)
                RETURN r
            "
        };

        if (culture.Parent != null)
        {
            queryList.Add(
                // Create Relationship Between Culture and Parent
                $@"
                        MATCH 
                            (c:Culture {{ Name: '{culture.Name}' }}), 
                            (p {{ Name: '{culture.Parent}' }})
                        MERGE
                            (c)-[r:HAS_PARENT]->(p)
                        RETURN r
                    ");
        }

        if (culture.Finished == true)
        {
            queryList.Add(
                // Create IsSuccessful Label on Culture
                $@"
                        MATCH (c:Culture {{ Name: '{culture.Name}' }})
                        SET c {culture.IsSuccessful()}
                        RETURN c
                    ");
        }
        else
        {
            queryList.Add(
                // Create InProgress Label on Culture
                $@"
                        MATCH (c:Culture {{ Name: '{culture.Name}' }})
                        SET c :InProgress
                        RETURN c
                    ");
        }

        if (culture.Vendor != null)
        {
            queryList.Add($@"
                MATCH 
                    (c:Culture {{ Name: '{culture.Name}'}} ),
                    (v:Vendor  {{ Name: '{culture.Vendor}' }})
                MERGE
                    (c)-[r:PURCHASED_FROM]->(v)
                RETURN 
                    r
            ");
        }

        return queryList;
    }
        
    public async Task DeleteById(string elementId)
    {
        var query = $"MATCH (c:Culture) WHERE elementId(c) = '{ elementId }' DETACH DELETE c RETURN c";
        var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(query);

        if(delete.ElementId != elementId)
            _logger.LogWarning("Node with elementId {ElementId} was not deleted, or was not found for deletion", elementId);
        
        _logger.LogInformation("Node with elementId {ElementId} was deleted successfully", elementId);
    }
        
    public async Task<string> Update(string elementId, Culture culture)
    {
        var query = $"MATCH (c:Culture) WHERE elementId(c) = '{elementId}' ";

        DateTime.TryParse(culture.ModifiedOn.ToString(), out var parsedDateTime);
        
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
                    (u:User {{ Name: '{culture.ModifiedBy}'}} )
                MERGE 
                    (u)-[r:MODIFIED]->(c)
                RETURN
                    r            
            "
        };
        
        // Update Name
        if (!string.IsNullOrEmpty(culture.Name))
            queryList.Add(query + $"SET c.Name = '{culture.Name}' RETURN c");

        // Update Strain
        if (!string.IsNullOrEmpty(culture.Strain))
        {
            queryList.Add($@"
                MATCH 
                    (c:Culture)-[r:HAS_STRAIN]->(:Strain)
                WHERE
                    elementId(c) = '{elementId}'
                DELETE 
                    r
                RETURN 
                    r
            ");
            
            queryList.Add($@"
                MATCH 
                    (c:Culture),
                    (s:Strain  {{ Name: '{culture.Strain}' }})
                WHERE
                    elementId(c) = '{elementId}'
                MERGE 
                    (c)-[r:HAS_STRAIN]->(s) 
                RETURN 
                    r
            ");
        }
        
        // Update Type
        if (!string.IsNullOrEmpty(culture.Type))
            queryList.Add(query + $"SET c.Type = '{culture.Type}' RETURN c");
        
        // Update Recipe
        // if (!string.IsNullOrEmpty(culture.Type))
        //   queryList.Add(query + $"SET c.Type = '{culture.Type}' RETURN c");
        
        // Update Location
        if (!string.IsNullOrEmpty(culture.Location))
        {
            queryList.Add($@"
                MATCH 
                    (c:Culture)-[r:STORED_IN]->(:Location)
                WHERE
                    elementId(c) = '{elementId}'
                DELETE 
                    r
                RETURN 
                    r
            ");
            
            queryList.Add($@"
                MATCH 
                    (c:Culture),
                    (l:Location  {{ Name: '{culture.Location}' }})
                WHERE
                    elementId(c) = '{elementId}'
                MERGE 
                    (c)-[r:STORED_IN]->(l) 
                RETURN 
                    r
            ");
        }
        
        // Update Parent
        if (string.IsNullOrEmpty(culture.Parent))
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
                RETURN  
                    r
            ");
        }
        else
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
                    (p {{Name: '{culture.Parent}' }})
                MERGE 
                    (c)-[r:HAS_PARENT]->(p) 
                RETURN 
                    r
            ");
        }
        
        // Update Child
        if (string.IsNullOrEmpty(culture.Child))
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
                RETURN  
                    r
            ");
        }
        else
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
                    (p {{Name: '{culture.Child}' }})
                MERGE 
                    (c)<-[r:HAS_PARENT]-(p) 
                RETURN 
                    r
            ");
        }
        
        // Update Vendor
        if (!string.IsNullOrEmpty(culture.Vendor))
        {
            queryList.Add($@"
                MATCH 
                    (c:Culture)
                WHERE
                    elementId(c) = '{elementId}'
                OPTIONAL MATCH
                    (c)-[r:PURCHASED_FROM]->(v)
                DELETE 
                    r
                WITH
                    c
                MATCH
                    (v:Vendor  {{ Name: '{culture.Vendor}' }})
                MERGE
                    (c)-[r:PURCHASED_FROM]->(v)
                RETURN 
                    r
            ");
        }
        
        // Update Successful + Finished
        // TODO
        
        var cultures = await _neo4JDataAccess.RunTransaction(queryList);
        return JsonConvert.SerializeObject(cultures);
    }
}