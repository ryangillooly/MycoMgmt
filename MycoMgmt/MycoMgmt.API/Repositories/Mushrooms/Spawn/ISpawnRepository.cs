using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.Mushrooms;
using Neo4j.Driver;

namespace MycoMgmt.API.Repositories
{
    public interface ISpawnRepository
    {
        public Task<List<IEntity>> Create(Spawn spawn);
        Task<string> SearchByName(Spawn spawn);
        Task<string> GetByName(Spawn spawn);
        public Task<string> GetById(Spawn spawn);
        public Task<string> GetAll(Spawn spawn, int skip, int limit);
        public Task Delete(Spawn spawn);
        public Task<string> Update(Spawn spawn);
    }
}