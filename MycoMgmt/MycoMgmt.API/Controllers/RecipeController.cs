using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Infrastructure.Repositories;
using MycoMgmt.Domain.Models;

namespace MycoMgmt.API.Controllers;

[Route("recipe")]
[ApiController]
public class RecipeController : Controller
{
    private readonly BaseRepository<Recipe> _recipeRepository;

    public RecipeController(BaseRepository<Recipe> repo)
    {
        _recipeRepository = repo;
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

        var result = await _recipeRepository.Create(recipe);
        
        return Created("", result);
    }

    [HttpPut("{elementId}")]
    public async Task<IActionResult> Update
    (
        string  elementId,
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
    
    [HttpDelete("{elementId}")]
    public async Task<IActionResult> Delete(string elementId)
    {
        await _recipeRepository.Delete(new Recipe { ElementId = elementId});
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await _recipeRepository.GetAll(new Recipe(), skip, limit));

    [HttpGet("id/{elementId}")]
    public async Task<IActionResult> GetById(string elementId) => Ok(await _recipeRepository.GetById(new Recipe { ElementId = elementId }));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name) => Ok(await _recipeRepository.GetByName(new Recipe { Name = name }));

    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name) => Ok(await _recipeRepository.SearchByName(new Recipe { Name = name }));

}