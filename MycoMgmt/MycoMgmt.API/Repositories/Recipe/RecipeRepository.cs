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

    public async Task<string> Create(Recipe recipe)
    {
        if (recipe == null || string.IsNullOrWhiteSpace(recipe.Name))
            throw new ArgumentNullException(nameof(recipe), "Recipe must not be null");

        var queryList = new List<string>
        {
            recipe.Create(),
            recipe.CreateIngredientRelationship(),
            recipe.CreateCreatedRelationship(),
            recipe.CreateCreatedOnRelationship()
        };
        
        queryList.RemoveAll(item => item is null);

        return await _neo4JDataAccess.RunTransaction(queryList);
    }
    
    public async Task Delete(string elementId)
    {
        var query = $"MATCH (r:Recipe) WHERE elementId(r) = '{ elementId }' DETACH DELETE r RETURN r";
        var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(query);

        if(delete.ElementId != elementId)
            _logger.LogWarning("Node with elementId {ElementId} was not deleted, or was not found for deletion", elementId);
        
        _logger.LogInformation("Node with elementId {ElementId} was deleted successfully", elementId);
    }
        
    public async Task<string> Update(string elementId, Recipe recipe)
    {
        var query = $"MATCH (r:Recipe) WHERE elementId(r) = '{elementId}' ";

        DateTime.TryParse(recipe.ModifiedOn.ToString(), out var parsedDateTime);
        
        var queryList = new List<string>
        {
            recipe.UpdateModifiedOnRelationship(elementId),
            recipe.UpdateModifiedRelationship(elementId)
        };
        
        // Update Name
        if (!string.IsNullOrEmpty(recipe.Name))
            queryList.Add(query + $"SET r.Name = '{recipe.Name}' RETURN r");

        // Update Type
        if (!string.IsNullOrEmpty(recipe.Type))
            queryList.Add(query + $"SET r.Type = '{recipe.Type}' RETURN r");

        // Update Ingredients
        if(recipe.Ingredients != null)
            queryList.Add(recipe.UpdateIngredientRelationship(elementId));
        
        // Update Steps
        if(recipe.Steps != null)
            queryList.Add(query + $"SET r.Steps = '{recipe.Steps.ToNumberedStringList()}'");

        // Update Description
        if(!string.IsNullOrEmpty(recipe.Description))
            queryList.Add(query + $"SET r.Description = '{recipe.Description}' RETURN r");
      
        // Update Notes
        if(!string.IsNullOrEmpty(recipe.Notes))
            queryList.Add(query + $"SET r.Notes = '{recipe.Notes}' RETURN r");
        
        queryList.RemoveAll(item => item is null);
        
        var spawnData = await _neo4JDataAccess.RunTransaction(queryList);
        return JsonConvert.SerializeObject(spawnData, Formatting.Indented);
    }
}