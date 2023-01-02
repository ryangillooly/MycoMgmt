using Microsoft.Extensions.Logging;
using MycoMgmt.Domain.Models;
using MycoMgmt.Infrastructure.DataStores.Neo4J;
using Neo4j.Driver;
using Newtonsoft.Json;
#pragma warning disable CS8604

// ReSharper disable once CheckNamespace
namespace MycoMgmt.Infrastructure.Repositories;

public class RecipeRepository : BaseRepository<Recipe>
{
    private readonly INeo4JDataAccess _neo4JDataAccess;
    private ILogger<RecipeRepository> _logger;
        
    public RecipeRepository(INeo4JDataAccess neo4JDataAccess, ILogger<RecipeRepository> logger)
    {
        _neo4JDataAccess = neo4JDataAccess;
        _logger = logger;
    }

    public override async Task<string> SearchByName(Recipe recipe)
    {
        var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(recipe.SearchByNameQuery(), "x");
        return JsonConvert.SerializeObject(result);
    }

    public override async Task<string> GetByName(Recipe recipe)
    {
        var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(recipe.GetByNameQuery(), "x");

        return JsonConvert.SerializeObject(result);
    }

    public override async Task<string> GetById(Recipe recipe)
    {
        var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(recipe.GetByIdQuery());
        return JsonConvert.SerializeObject(result);
    }
    
    public override async Task<string> GetAll(Recipe recipe, int skip, int limit)
    {
        var result = await _neo4JDataAccess.ExecuteReadListAsync(recipe.GetAllQuery(skip, limit), "x");
        return JsonConvert.SerializeObject(result);
    }
    
    public override async Task<List<IEntity>> Create(Recipe recipe)
    {
        var queryList = recipe.CreateQueryList();        
        return await _neo4JDataAccess.RunTransaction(queryList);
    }
    
    public override async Task Delete(Recipe recipe)
    {
        var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(recipe.Delete());

        if(delete.ElementId != recipe.ElementId)
            _logger.LogWarning("Node with elementId {ElementId} was not deleted, or was not found for deletion", recipe.ElementId);
        
        _logger.LogInformation("Node with elementId {ElementId} was deleted successfully", recipe.ElementId);
    }
        
    public override async Task<string> Update(Recipe recipe)
    {
        var queryList = recipe.UpdateQueryList();   
        var results = await _neo4JDataAccess.RunTransaction(queryList);
        return JsonConvert.SerializeObject(results);
    }
}