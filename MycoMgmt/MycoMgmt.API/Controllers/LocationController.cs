using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Repositories;
using MycoMgmt.Domain.Models;
using Neo4j.Driver;
using Newtonsoft.Json;

namespace MycoMgmt.API.Controllers
{
    [Route("location")]
    [ApiController]
    public class LocationController : Controller
    {
        private readonly ILocationsRepository _locationsRepository;
        
        public LocationController(ILocationsRepository repo, IDriver driver)
        {
            _locationsRepository = repo;
        }
        
        [HttpPost]
        public async Task<IActionResult> Create
        (
            string name,
            bool?  agentConfigured,
            string createdOn,
            string createdBy
        )
        {
            var location = new Location()
            {
                Name       = name,
                CreatedOn  = DateTime.Parse(createdOn),
                CreatedBy  = createdBy
            };

            if (agentConfigured != null)
                location.AgentConfigured = agentConfigured;

            var resultList = new List<string>
            {
                await _locationsRepository.Create(location)
            };   
            
            return Created("", string.Join(",", resultList));
        }
        
        [HttpPut("{elementId}")]
    public async Task<IActionResult> Update
    (
        string  elementId,
        string? name,
        bool?   agentConfigured,
        string  modifiedOn,
        string  modifiedBy
    )
    {
        var location = new Location
        {
            ModifiedOn = DateTime.Parse(modifiedOn),
            ModifiedBy = modifiedBy
        };
        

        if (name != null)
            location.Name = name;
        
        if (agentConfigured != null)
            location.AgentConfigured = agentConfigured;
        
        
        var result = await _locationsRepository.Update(location, elementId);

        return Ok(result);
    }
        
        [HttpGet]
        public async Task<string> GetAllLocations(string name)
        {
            var node = await _locationsRepository.GetAll();
            return (node is null ? null : JsonConvert.SerializeObject(node)) ?? string.Empty;
        }
    }
}