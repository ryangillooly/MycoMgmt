using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models;

namespace MycoMgmt.API.Repositories
{
    public interface ILocationsRepository
    {
        [HttpPost]
        Task<string> Add(Location location);
        
        [HttpGet]
        public Task<List<Dictionary<string, object>>> GetAll();
    }
}