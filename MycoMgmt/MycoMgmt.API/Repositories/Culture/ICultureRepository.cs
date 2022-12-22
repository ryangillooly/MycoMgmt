using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.Mushrooms;
using Neo4j.Driver;

namespace MycoMgmt.API.Repositories
{
    public interface ICultureRepository
    {
        public Task<string> Create(Culture culture);
        Task<string> SearchByName(string name);
        Task<string> GetByName(string name);
        public Task<long> GetCount();
        public Task<string> GetById(string id);
        public Task<string> GetAll();
        public Task DeleteById(string elementId);
        public Task<string> Update(string elementId, Culture culture);
    }
}