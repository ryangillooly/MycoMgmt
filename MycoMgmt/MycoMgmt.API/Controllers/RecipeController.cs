using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Repositories;
using MycoMgmt.Domain.Models;
using MycoMgmt.Domain.Models.Mushrooms;

namespace MycoMgmt.API.Controllers;

[Route("recipe")]
[ApiController]
public class RecipeController : Controller
{
    private readonly IRecipeRepository _recipeRepository;

    public RecipeController(IRecipeRepository repo)
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
            Name      = name,
            Type      = type,
            CreatedOn = DateTime.Parse(createdOn),
            CreatedBy = createdBy
        };

        if (notes != null)
            recipe.Notes = notes;

        if (description != null)
            recipe.Description = description;

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
            ModifiedOn = DateTime.Parse(modifiedOn),
            ModifiedBy = modifiedBy
        };
        
        if (name != null)
            recipe.Name = name;

        if (type != null)
            recipe.Type = type;
        
        if (notes != null)
            recipe.Notes = notes;

        if (description != null)
            recipe.Description = description;

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
        
        var result = await _recipeRepository.Update(elementId, recipe);

        return Ok(result);
    }
    

    [HttpDelete("{elementId}")]
    public async Task<IActionResult> Delete(string elementId)
    {
        await _recipeRepository.Delete(elementId);
        return NoContent();
    }
}