using MycoMgmt.API.DataStores.Neo4J;
using MycoMgmt.API.Helpers;
using MycoMgmt.Domain.Models;
using Neo4j.Driver;
using Newtonsoft.Json;
#pragma warning disable CS8604

// ReSharper disable once CheckNamespace
namespace MycoMgmt.API.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly INeo4JDataAccess _neo4JDataAccess;
    private ILogger<RecipeRepository> _logger;
        
    public RecipeRepository(INeo4JDataAccess neo4JDataAccess, ILogger<RecipeRepository> logger)
    {
        _neo4JDataAccess = neo4JDataAccess;
        _logger = logger;
    }

    public async Task<string> SearchByName(Recipe recipe)
    {
        var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(recipe.SearchByNameQuery(), "x");
        return JsonConvert.SerializeObject(result);
    }

    public async Task<string> GetByName(Recipe recipe)
    {
        var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(recipe.GetByNameQuery(), "x");

        return JsonConvert.SerializeObject(result);
    }

    public async Task<string> GetById(Recipe recipe)
    {
        var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(recipe.GetByIdQuery());
        return JsonConvert.SerializeObject(result);
    }
    
    public async Task<string> GetAll(Recipe recipe)
    {
        var result = await _neo4JDataAccess.ExecuteReadListAsync(recipe.GetAll(), "x");
        return JsonConvert.SerializeObject(result);
    }
    
    public async Task<string> Create(Recipe recipe)
    {
        if (recipe == null || string.IsNullOrWhiteSpace(recipe.Name))
            throw new ArgumentNullException(nameof(recipe), "Recipe must not be null");

        var queryList = new List<string?>
        {
            recipe.Create(),
            recipe.CreateIngredientRelationship(),
            recipe.CreateCreatedRelationship(),
            recipe.CreateCreatedOnRelationship()
        };
        
        return await _neo4JDataAccess.RunTransaction(queryList);
    }
    
    public async Task Delete(Recipe recipe)
    {
        var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(recipe.Delete());

        if(delete.ElementId != recipe.ElementId)
            _logger.LogWarning("Node with elementId {ElementId} was not deleted, or was not found for deletion", recipe.ElementId);
        
        _logger.LogInformation("Node with elementId {ElementId} was deleted successfully", recipe.ElementId);
    }
        
    public async Task<string> Update(Recipe recipe)
    {
        var queryList = new List<string?>
        {
            recipe.UpdateName(),
            recipe.UpdateType(),
            recipe.UpdateSteps(),
            recipe.UpdateDescription(),
            recipe.UpdateNotes(),
            recipe.UpdateIngredientRelationship(),
            recipe.UpdateModifiedOnRelationship(),
            recipe.UpdateModifiedRelationship()
        };
        
        var results = await _neo4JDataAccess.RunTransaction(queryList);
        return JsonConvert.SerializeObject(results);
    }
}