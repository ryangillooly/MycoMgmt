using MycoMgmt.Domain.Models;

namespace MycoMgmt.Infrastructure.Repositories
{
    public interface IRecipeRepository
    {
        public Task<string> Create(Recipe recipe);
        public Task Delete(Recipe recipe);
        public Task<string> Update(Recipe recipe);
        Task<string> SearchByName(Recipe recipe);
        Task<string> GetByName(Recipe recipe);
        public Task<string> GetById(Recipe recipe);
        public Task<string> GetAll(Recipe recipe, int skip, int limit);
    }
}