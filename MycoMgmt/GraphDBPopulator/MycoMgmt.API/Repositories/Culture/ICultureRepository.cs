using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Models;
using Neo4j.Driver;

namespace MycoMgmt.API.Repositories
{
    public interface ICultureRepository
    {
        [HttpPost]
        public Task<string> AddCulture(Culture culture);
        public Task<List<Dictionary<string, object>>> SearchCultureByName(string searchString);
        public Task<string> SearchCultureById(string id);
        public Task<long> GetCultureCount();
    }
}