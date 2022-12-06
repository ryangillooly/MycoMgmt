
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Models;
using MycoMgmt.API.Repositories;

namespace MycoMgmt.API.Controllers
{
    [Route("api/recipes")]
    [ApiController]
    public class RecipeController : Controller
    {
        private readonly IRecipeRepository _repo;
        
        public RecipeController(IRecipeRepository repo)
        {
            this._repo = repo;
        }

        [HttpPost("new")]
        public async void NewRecipe(string name, RecipeTypes type, string desc, string steps)
        {
            var recipe = new Recipe()
            {
                Id          = Guid.NewGuid(),
                Name        = name,
                Type        = type,
                Description = desc,
                Steps       = steps
            };

            await _repo.AddRecipe(recipe);
        }

        [HttpGet("count")]
        public async Task<long> GetRecipeCount()
        {
            var recipeCount = await _repo.GetRecipeCount();
            Console.WriteLine($"RecipeCount - { recipeCount.ToString() }");
            return recipeCount;
        }
    }
}
