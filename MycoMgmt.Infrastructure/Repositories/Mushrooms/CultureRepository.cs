using Microsoft.Extensions.Logging;
using MycoMgmt.Domain.Models;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.Infrastructure.DataStores.Neo4J;
using Neo4j.Driver;
using Newtonsoft.Json;
#pragma warning disable CS8604

// ReSharper disable once CheckNamespace
namespace MycoMgmt.Infrastructure.Repositories;

public class CultureRepository
{
    private readonly INeo4JDataAccess _neo4JDataAccess;
    private ILogger<CultureRepository> _logger;

    public CultureRepository(INeo4JDataAccess neo4JDataAccess, ILogger<CultureRepository> logger)
    {
        _neo4JDataAccess = neo4JDataAccess;
        _logger = logger;
    }

    public async Task<List<IEntity>> Create(Culture culture)
    {
        var queryList = culture.CreateQueryList();
        var result = await _neo4JDataAccess.RunTransaction(queryList);
        
        return result;
    }
    public async Task<string> Update(Culture culture)
    {
        var queryList = culture.UpdateQueryList();
        var results = await _neo4JDataAccess.RunTransaction(queryList);
        return JsonConvert.SerializeObject(results);
    }
    
    public async Task Delete(Culture culture)
    {
        var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(culture.Delete());
        
        if(delete.Id.ToString() == culture.Id)
            _logger.LogInformation("Node with Id {Id} was deleted successfully", culture.Id);
        else
            _logger.LogWarning("Node with Id {Id} was not deleted, or was not found for deletion", culture.Id);
    }

    public async Task<string> SearchByName(Culture culture)
    {
        var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(culture.SearchByNameQuery(), "x");
        return JsonConvert.SerializeObject(result);
    }

    public async Task<string> GetByName(Culture culture)
    {
        var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(culture.GetByNameQuery(), "x");

        return JsonConvert.SerializeObject(result);
    }

    public async Task<string> GetById(Culture culture)
    {
        var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(culture.GetByIdQuery());
        return JsonConvert.SerializeObject(result);
    }
    
    public async Task<string> GetAll(Culture culture, int skip, int limit)
    {
        var result = await _neo4JDataAccess.ExecuteReadListAsync(culture.GetAllQuery(skip, limit), "result");
        return JsonConvert.SerializeObject(result);
    }
}