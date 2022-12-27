using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    
    public async Task<string> Create(Culture culture)
    {
        if (culture == null || string.IsNullOrWhiteSpace(culture.Name))
            throw new ArgumentNullException(nameof(culture), "Culture must not be null");

        var queryList = new List<string>
        {
            culture.Create(),
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

        queryList.RemoveAll(item => item is null);
           
        return await _neo4JDataAccess.RunTransaction(queryList);
    }
    
    public async Task Delete(string elementId)
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

        var queryList = new List<string>
        {
            culture.UpdateModifiedOnRelationship(elementId),
            culture.UpdateModifiedRelationship(elementId),
            culture.UpdateStatus(elementId)
        };
        
        // Update Name
        if (!string.IsNullOrEmpty(culture.Name))
            queryList.Add(query + $"SET c.Name = '{culture.Name}' RETURN c");

        // Update Type
        if (!string.IsNullOrEmpty(culture.Type))
            queryList.Add(query + $"SET c.Type = '{culture.Type}' RETURN c");
                
        // Update Notes
        if(!string.IsNullOrEmpty(culture.Notes))
            queryList.Add(query + $"SET c.Notes = '{culture.Notes}' RETURN c");
        
        // Update Recipe
        if (!string.IsNullOrEmpty(culture.Recipe))
            queryList.Add(culture.UpdateRecipeRelationship(elementId));
        
        // Update Strain
        if (!string.IsNullOrEmpty(culture.Strain))
            queryList.Add(culture.UpdateStrainRelationship(elementId));
        
        // Update Location
        if (!string.IsNullOrEmpty(culture.Location))
            queryList.Add(culture.UpdateLocationRelationship(elementId));
        
        // Update Parent
        if (culture.Parent != null)
            queryList.Add(culture.UpdateParentRelationship(elementId));
        
        // Update Child
        if (culture.Child != null)
            queryList.Add(culture.UpdateChildRelationship(elementId));
        
        // Update Vendor
        if (!string.IsNullOrEmpty(culture.Vendor))
            queryList.Add(culture.UpdateVendorRelationship(elementId));

        queryList.RemoveAll(item => item is null);
        
        var cultures = await _neo4JDataAccess.RunTransaction(queryList);
        return JsonConvert.SerializeObject(cultures, Formatting.Indented);
    }
}