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

    public async Task<string> SearchByName(string name)
    {
        var query = $"MATCH (s:Bulk) WHERE toUpper(s.Name) CONTAINS toUpper('{ name }') RETURN s {{ Name: s.Name, Type: s.Type }} ORDER BY s.Name LIMIT 5";
        var bulk = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "c");

        return JsonConvert.SerializeObject(bulk);
    }

    public async Task<string> GetByName(string name)
    {
        var query = $"MATCH (s:Bulk) WHERE toUpper(s.Name) = toUpper('{ name }') RETURN s {{ Name: s.Name, Type: s.Type }} ORDER BY s.Name LIMIT 5";
        var bulk = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "s");

        return JsonConvert.SerializeObject(bulk);
    }

    public async Task<string> GetById(string id)
    {
        var query = $"MATCH (b:Bulk) WHERE elementId(b) = '{id}' RETURN b";
        
            var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(query);
            return JsonConvert.SerializeObject(result);
    }
    
    public async Task<string> GetAll()
    {
        const string query = "MATCH (b:Bulk) RETURN b ORDER BY b.Name ASC";
        var bulk = await _neo4JDataAccess.ExecuteReadListAsync(query, "s");
        return JsonConvert.SerializeObject(bulk);
    }

    public async Task<long> GetCount()
    {
        const string query = "Match (b:Bulk) RETURN count(b) as BulkCount";
        var count = await _neo4JDataAccess.ExecuteReadScalarAsync<long>(query);
        return count;
    }
    
    public async Task<string> Create(Bulk bulk)
    {
        if (bulk == null || string.IsNullOrWhiteSpace(bulk.Name))
            throw new ArgumentNullException(nameof(bulk), "Bulk must not be null");

        var queryList = new List<string>
        {
            bulk.Create(),
            bulk.CreateStrainRelationship(),
            bulk.CreateLocationRelationship(),
            bulk.CreateCreatedRelationship(),
            bulk.CreateCreatedOnRelationship(),
            bulk.CreateParentRelationship(),
            bulk.CreateChildRelationship(),
            bulk.CreateNodeLabels()
        };

        queryList.RemoveAll(item => item is null);

        return await _neo4JDataAccess.RunTransaction(queryList);
    }
    
    public async Task Delete(string elementId)
    {
        var query = $"MATCH (b:Bulk) WHERE elementId(b) = '{ elementId }' DETACH DELETE b RETURN b";
        var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(query);

        if(delete.ElementId != elementId)
            _logger.LogWarning("Node with elementId {ElementId} was not deleted, or was not found for deletion", elementId);
        
        _logger.LogInformation("Node with elementId {ElementId} was deleted successfully", elementId);
    }
        
    public async Task<string> Update(string elementId, Bulk bulk)
    {
        var query = $"MATCH (b:Bulk) WHERE elementId(b) = '{elementId}' ";

        DateTime.TryParse(bulk.ModifiedOn.ToString(), out var parsedDateTime);
        
        var queryList = new List<string>
        {
            bulk.UpdateModifiedOnRelationship(elementId),
            bulk.UpdateModifiedRelationship(elementId),
            bulk.UpdateStatus(elementId)
        };
        
        // Update Recipe
        // TODO
        
        // Update Name
        if (!string.IsNullOrEmpty(bulk.Name))
            queryList.Add(query + $"SET b.Name = '{bulk.Name}' RETURN b");
        
        // Update Notes
        if (!string.IsNullOrEmpty(bulk.Name))
            queryList.Add(query + $"SET b.Notes = '{bulk.Notes}' RETURN b");
        
        // Update Location
        if (!string.IsNullOrEmpty(bulk.Location))
            queryList.Add(bulk.UpdateLocationRelationship(elementId));
        
        // Update Parent
        if (bulk.Parent != null)
            queryList.Add(bulk.UpdateParentRelationship(elementId));
        
        // Update Child
        if (bulk.Child != null)
            queryList.Add(bulk.UpdateChildRelationship(elementId));

        queryList.RemoveAll(item => item is null);
        
        var bulkData = await _neo4JDataAccess.RunTransaction(queryList);
        return JsonConvert.SerializeObject(bulkData);
    }
}