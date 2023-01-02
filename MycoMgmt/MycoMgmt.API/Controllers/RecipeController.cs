﻿using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Core.Helpers;
using MycoMgmt.Infrastructure.Repositories;
using MycoMgmt.Domain.Models;
using MycoMgmt.Infrastructure.Helpers;
using ILogger = Neo4j.Driver.ILogger;

namespace MycoMgmt.API.Controllers;

[Route("recipe")]
[ApiController]
public class RecipeController : Controller
{
    private readonly ActionRepository _recipeRepository;
    private readonly ILogger<RecipeController> _logger;

    public RecipeController(ActionRepository repo, ILogger<RecipeController> logger)
    {
        _recipeRepository = repo;
        _logger = logger;
    }
    
    [HttpPost]
    public async Task<IActionResult> Create
    (
        string  name,
        string  type,
        string? notes,
        string? description,
        string? steps, 
        string? ingredients,
        string  createdOn,
        string  createdBy
    )
    {
        var recipe = new Recipe()
        {
            Name        = name,
            Notes       = notes,
            Description = description,
            Type        = type,
            CreatedOn   = DateTime.Parse(createdOn),
            CreatedBy   = createdBy
        };

        recipe.SplitStepsToList(steps);
        recipe.SplitIngredientsToList(ingredients);
        
        var result = await _recipeRepository.CreateEntities(_logger, recipe);
            
        return Created("", result);
    }

    [HttpPut("{Id}")]
    public async Task<IActionResult> Update
    (
        string  Id,
        string? name,
        string? type,
        string? notes,
        string? description,
        string? steps,
        string? ingredients,
        string  modifiedOn,
        string  modifiedBy
    )
    {
        var recipe = new Recipe()
        {
            Name        = name,
            Type        = type,
            Notes       = notes,
            Description = description,
            ModifiedOn  = DateTime.Parse(modifiedOn),
            ModifiedBy  = modifiedBy
        };
        
        if (steps != null)
        {
            recipe.Steps = steps.Split(",").ToList();
            
            for (var i = 0; i < recipe.Steps.Count; i++)
            {
                recipe.Steps[i] = recipe.Steps[i].Trim();
            }
        }

        if (ingredients != null)
        {
            recipe.Ingredients = ingredients.Split(",").ToList();

            for (var i = 0; i < recipe.Ingredients.Count; i++)
            {
                recipe.Ingredients[i] = recipe.Ingredients[i].Trim();
            }
        }
        
        return Ok(await _recipeRepository.Update(recipe));
    }
    
    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete(string Id)
    {
        await _recipeRepository.Delete(new Recipe { Id = Id});
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await _recipeRepository.GetAll(new Recipe(), skip, limit));

    [HttpGet("id/{Id}")]
    public async Task<IActionResult> GetById(string Id) => Ok(await _recipeRepository.GetById(new Recipe { Id = Id }));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name) => Ok(await _recipeRepository.GetByName(new Recipe { Name = name }));

    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name) => Ok(await _recipeRepository.SearchByName(new Recipe { Name = name }));

}