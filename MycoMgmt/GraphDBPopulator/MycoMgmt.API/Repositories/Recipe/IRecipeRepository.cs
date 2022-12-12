using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MycoMgmt.API.Repositories.Recipe
{
    public interface IRecipeRepository
    {
        [HttpPost]
        public Task<string> AddRecipe(Models.Recipe recipe);
        public Task<List<Dictionary<string, object>>> SearchRecipeByName(string searchString);
        public Task<long> GetRecipeCount();
    }
}