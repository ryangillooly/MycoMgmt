using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.Mushrooms;
using Neo4j.Driver;

namespace MycoMgmt.API.Repositories
{
    public interface ISpawnRepository
    {
        public Task<string> Create(Spawn spawn);
        Task<string> SearchByName(Spawn spawn);
        Task<string> GetByName(Spawn spawn);
        public Task<string> GetById(Spawn spawn);
        public Task<string> GetAll();
        public Task Delete(string elementId);
        public Task<string> Update(string elementId, Spawn spawn);
    }
}