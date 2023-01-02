using System;
using Microsoft.Extensions.Logging;
using MycoMgmt.Domain.Models;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.Infrastructure.DataStores.Neo4J;
using Neo4j.Driver;
using Newtonsoft.Json;
#pragma warning disable CS8604

// ReSharper disable once CheckNamespace
namespace MycoMgmt.Infrastructure.Repositories;

public class BulkRepository : BaseRepository<Bulk>
{
    private readonly INeo4JDataAccess _neo4JDataAccess;
    private ILogger<BulkRepository> _logger;
        
    public BulkRepository(INeo4JDataAccess neo4JDataAccess, ILogger<BulkRepository> logger)
    {
        _neo4JDataAccess = neo4JDataAccess;
        _logger = logger;
    }
    
    public override async Task<List<IEntity>> Create(Bulk bulk)
    {
        var queryList = bulk.CreateQueryList();
        return await _neo4JDataAccess.RunTransaction(queryList);
    }
    
    public override async Task<string> Update(Bulk bulk)
    {
        var queryList = bulk.UpdateQueryList();
        var results = await _neo4JDataAccess.RunTransaction(queryList);
        return JsonConvert.SerializeObject(results);
    }
    
    public override async Task Delete(Bulk bulk)
    {
        var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(bulk.Delete());

        if(delete.Id.ToString() == bulk.Id)
            _logger.LogInformation("Node with Id {Id} was deleted successfully", bulk.Id);
        else    
            _logger.LogWarning("Node with Id {Id} was not deleted, or was not found for deletion", bulk.Id);
        
    }

    public override async Task<string> SearchByName(Bulk bulk)
    {
        var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(bulk.SearchByNameQuery(), "x");
        return JsonConvert.SerializeObject(result);
    }

    public override async Task<string> GetByName(Bulk bulk)
    {
        var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(bulk.GetByNameQuery(), "x");

        return JsonConvert.SerializeObject(result);
    }

    public override async Task<string> GetById(Bulk bulk)
    {
        var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(bulk.GetByIdQuery());
        return JsonConvert.SerializeObject(result);
    }
    
    public override async Task<string> GetAll(Bulk bulk, int skip, int limit)
    {
        var result = await _neo4JDataAccess.ExecuteReadListAsync(bulk.GetAllQuery(skip, limit), "result");
        return JsonConvert.SerializeObject(result);
    }
}