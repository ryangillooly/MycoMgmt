using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Repositories;
using MycoMgmt.Domain.Models;
using Neo4j.Driver;
using Newtonsoft.Json;

namespace MycoMgmt.API.Controllers
{
    [Route("locations")]
    [ApiController]
    public class LocationController : Controller
    {
        private readonly ILocationsRepository _locationsRepository;
        
        public LocationController(ILocationsRepository repo, IDriver driver)
        {
            _locationsRepository = repo;
        }
        
        [HttpPost("new")]
        public async Task<string> NewLocation
        (
            string name,
            string createdOn,
            string createdBy,
            string? modifiedOn,
            string? modifiedBy
        )
        {
            var location = new Location()
            {
                Name       = name,
                CreatedOn  = DateTime.Parse(createdOn),
                CreatedBy  = createdBy
            };

            if (modifiedOn != null)
                location.ModifiedOn = DateTime.Parse(modifiedOn);
            
            if(modifiedBy != null)
                location.ModifiedBy = modifiedBy;

            var result = await _locationsRepository.Add(location);
            return result;
        }
        
        [HttpGet("all")]
        public async Task<string> GetAllLocations(string name)
        {
            var node = await _locationsRepository.GetAll();
            return node is null ? null : JsonConvert.SerializeObject(node);
        }
    }
}