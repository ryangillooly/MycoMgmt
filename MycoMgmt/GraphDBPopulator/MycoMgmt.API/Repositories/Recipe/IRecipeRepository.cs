using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models;

namespace MycoMgmt.API.Repositories.Recipe
{
    public interface IRecipeRepository
    {
        [HttpPost]
        public Task<string> Add(Domain.Models.Recipe recipe);
        public Task<List<Dictionary<string, object>>> SearchByName(string searchString);
        public Task<long> GetCount();
    }
}