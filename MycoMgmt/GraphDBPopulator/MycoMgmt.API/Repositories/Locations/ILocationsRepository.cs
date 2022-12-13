using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Models;

namespace MycoMgmt.API.Repositories
{
    public interface ILocationsRepository
    {
        [HttpPost]
        Task<string> AddLocation(Location location);
        
        [HttpGet]
        public Task<List<Dictionary<string, object>>> GetAllLocations();
    }
}