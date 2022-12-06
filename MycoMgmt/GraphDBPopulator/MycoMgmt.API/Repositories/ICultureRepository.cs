using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Models;

namespace MycoMgmt.API.Repositories
{
    public interface ICultureRepository
    {
        [HttpPost]
        public Task<bool> AddCulture(Culture culture);
        public Task<List<Dictionary<string, object>>> SearchCultureByName(string searchString);
        public Task<long> GetCultureCount();
    }
}