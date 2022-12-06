using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Models;

namespace MycoMgmt.API.Repositories
{
    public interface IRecipeRepository
    {
        [HttpPost]
        public Task<bool> AddRecipe(Recipe recipe);
        public Task<List<Dictionary<string, object>>> SearchRecipeByName(string searchString);
        public Task<long> GetRecipeCount();
    }
}