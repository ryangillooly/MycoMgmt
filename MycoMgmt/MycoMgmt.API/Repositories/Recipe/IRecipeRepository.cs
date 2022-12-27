using MycoMgmt.Domain.Models;

namespace MycoMgmt.API.Repositories
{
    public interface IRecipeRepository
    {
        public Task<string> Create(Recipe recipe);
        public Task Delete(string elementId);
        public Task<string> Update(string elementId, Recipe recipe);
    }
}