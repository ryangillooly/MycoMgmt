using Microsoft.Extensions.Logging;
using MycoMgmt.Domain.Models;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.Infrastructure.DataStores.Neo4J;
using Neo4j.Driver;
using Newtonsoft.Json;

#pragma warning disable CS8604

// ReSharper disable once CheckNamespace
namespace MycoMgmt.Infrastructure.Repositories;

public class FruitRepository : BaseRepository<Fruit>
{
    private readonly INeo4JDataAccess _neo4JDataAccess;
    private ILogger<FruitRepository> _logger;
        
    public FruitRepository(INeo4JDataAccess neo4JDataAccess, ILogger<FruitRepository> logger)
    {
        _neo4JDataAccess = neo4JDataAccess;
        _logger = logger;
    }

    
    public override async Task<List<IEntity>> Create(Fruit fruit)
    {
        var queryList = fruit.CreateQueryList();
        return await _neo4JDataAccess.RunTransaction(queryList);
    }
    
    public override async Task Delete(Fruit fruit)
    {
        var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(fruit.Delete());
        
        if(delete.Id.ToString() == fruit.Id)
            _logger.LogInformation("Node with Id {Id} was deleted successfully", fruit.Id);
        else
            _logger.LogWarning("Node with Id {Id} was not deleted, or was not found for deletion", fruit.Id);
    }
        
    public override async Task<string> Update(Fruit fruit)
    {
        var queryList = fruit.CreateQueryList();
        var results = await _neo4JDataAccess.RunTransaction(queryList);
        return JsonConvert.SerializeObject(results);
    }
    
    public override async Task<string> SearchByName(Fruit fruit)
    {
        var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(fruit.SearchByNameQuery(), "x");
        return JsonConvert.SerializeObject(result);
    }

    public override async Task<string> GetByName(Fruit fruit)
    {
        var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(fruit.GetByNameQuery(), "x");
        return JsonConvert.SerializeObject(result);
    }

    public override async Task<string> GetById(Fruit fruit)
    {
        var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(fruit.GetByIdQuery());
        return JsonConvert.SerializeObject(result);
    }
    
    public override async Task<string> GetAll(Fruit fruit, int skip, int limit)
    {
        var result = await _neo4JDataAccess.ExecuteReadListAsync(fruit.GetAllQuery(skip, limit), "result");
        var dedupeResult = result.OfType<Dictionary<string, object>>().Distinct(new DictionaryEqualityComparer("Id")).ToList();
        return JsonConvert.SerializeObject(dedupeResult);
    }
}

public class DictionaryEqualityComparer : IEqualityComparer<Dictionary<string, object>>
{
    private readonly string _key;

    public DictionaryEqualityComparer(string key)
    {
        _key = key;
    }

    public bool Equals(Dictionary<string, object> x, Dictionary<string, object> y)
    {
        // Compare the values of the specified key for both dictionaries
        return x[_key].Equals(y[_key]);
    }

    public int GetHashCode(Dictionary<string, object> obj)
    {
        // Generate a hash code for the value of the specified key
        return obj[_key].GetHashCode();
    }
}