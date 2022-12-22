using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.Mushrooms;

namespace MycoMgmt.API.Repositories
{
    public interface ICultureRepository
    {
        [HttpPost]
        public Task<string> Add(Culture culture);
        Task<List<Dictionary<string, object>>> SearchByName(string name);
        Task<List<Dictionary<string, object>>> GetByName(string name);
        public Task<long> GetCount();
        public Task<string> GetById(string id);
        public Task<List<string>> GetAll();
        Task<string> Test();
    }
}