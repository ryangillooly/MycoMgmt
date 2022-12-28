using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Repositories;
using MycoMgmt.Domain.Models;
using Neo4j.Driver;

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
                Name            = name,
                AgentConfigured = agentConfigured,
                CreatedOn       = DateTime.Parse(createdOn),
                CreatedBy       = createdBy
            };
            
            return Created("", await _locationsRepository.Create(location));
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
                ElementId       = elementId,
                Name            = name,
                AgentConfigured = agentConfigured,
                ModifiedOn      = DateTime.Parse(modifiedOn),
                ModifiedBy      = modifiedBy
            };
            
            return Ok(await _locationsRepository.Update(location));
        }

        [HttpDelete("{elementId}")]
        public async Task<IActionResult> Delete(string elementId)
        {
            await _locationsRepository.Delete(new Location { ElementId = elementId });
            return NoContent();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll(int? skip, int? limit) => Ok(await _locationsRepository.GetAll(new Location(), skip, limit));
        
        [HttpGet("id/{elementId}")]
        public async Task<IActionResult> GetById(string elementId) => Ok(await _locationsRepository.GetById(new Location { ElementId = elementId }));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await _locationsRepository.GetByName(new Location { Name = name }));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await _locationsRepository.SearchByName(new Location { Name = name }));

    }
}