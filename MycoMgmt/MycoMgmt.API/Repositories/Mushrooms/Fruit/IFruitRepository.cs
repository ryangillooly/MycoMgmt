using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.Mushrooms;
using Neo4j.Driver;

namespace MycoMgmt.API.Repositories
{
    public interface IFruitRepository
    {
        public Task<List<IEntity>> Create(Fruit fruit);
        Task<string> SearchByName(Fruit fruit);
        Task<string> GetByName(Fruit fruit);
        public Task<string> GetById(Fruit fruit);
        public Task<string> GetAll(Fruit fruit, int skip, int limit);
        public Task Delete(Fruit fruit);
        public Task<string> Update(Fruit fruit);
    }
}