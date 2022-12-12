using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Models.Mushrooms;

namespace MycoMgmt.API.Repositories
{
    public interface ICultureRepository
    {
        [HttpPost]
        public Task<string> AddCulture(Culture culture);
        Task<List<Dictionary<string, object>>> SearchCulturesByName(string name);
        Task<List<Dictionary<string, object>>> GetCultureByName(string name);
        public Task<long> GetCultureCount();
        public Task<string> GetCultureById(string id);
    }
}