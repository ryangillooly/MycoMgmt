using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Repositories.Recipe;
using MycoMgmt.Domain.Models;

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
        public async void NewRecipe(string name, string type, string desc, string steps)
        {
            var recipe = new Recipe()
            {
                Name        = name,
                Type        = type,
                Description = desc,
                Steps       = steps
            };

            await _repo.Add(recipe);
        }

        [HttpGet("count")]
        public async Task<long> GetRecipeCount()
        {
            var recipeCount = await _repo.GetCount();
            Console.WriteLine($"RecipeCount - { recipeCount.ToString() }");
            return recipeCount;
        }
    }
}
