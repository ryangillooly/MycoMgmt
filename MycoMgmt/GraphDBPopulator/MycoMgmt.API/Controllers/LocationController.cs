using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MycoMgmt.API.Models;
using MycoMgmt.API.Models.Mushrooms;
using MycoMgmt.API.Repositories;
using Neo4j.Driver;
using Newtonsoft.Json;

namespace MycoMgmt.API.Controllers
{
    [Route("api/locations")]
    [ApiController]
    public class LocationController : Controller
    {
        private readonly ILocationsRepository _locationsRepository;
        private readonly IDriver _driver;
        
        public LocationController(ILocationsRepository repo, IDriver driver)
        {
            this._locationsRepository = repo;
            this._driver = driver;
        }
        
        [HttpGet("all")]
        public async Task<string> GetAllLocations(string name)
        {
            var node = await _locationsRepository.GetAllLocations();
            return node is null ? null : JsonConvert.SerializeObject(node);
        }
    }
}