using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MycoMgmt.API.Repositories
{
    public interface ILocationsRepository
    {
        [HttpPost]
        public Task<List<Dictionary<string, object>>> GetAllLocations();
    }
}