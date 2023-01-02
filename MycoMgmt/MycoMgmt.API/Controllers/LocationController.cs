using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Infrastructure.Repositories;
using MycoMgmt.Domain.Models;
using MycoMgmt.Infrastructure.Helpers;
using Neo4j.Driver;

namespace MycoMgmt.API.Controllers
{
    [Route("location")]
    [ApiController]
    public class LocationController : Controller
    {
        private readonly BaseRepository<Location> _locationsRepository;
        private readonly ILogger<LocationController> _logger;
        
        public LocationController(BaseRepository<Location> repo, ILogger<LocationController> logger)
        {
            _locationsRepository = repo;
            _logger = logger;
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
            
            var result = await _locationsRepository.CreateEntities(_logger, location);
            
            return Created("", result);
        }
        
        [HttpPut("{Id}")]
        public async Task<IActionResult> Update
        (
            string  Id,
            string? name,
            bool?   agentConfigured,
            string  modifiedOn,
            string  modifiedBy
        )
        {
            var location = new Location
            {
                Id       = Id,
                Name            = name,
                AgentConfigured = agentConfigured,
                ModifiedOn      = DateTime.Parse(modifiedOn),
                ModifiedBy      = modifiedBy
            };
            
            return Ok(await _locationsRepository.Update(location));
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(string Id)
        {
            await _locationsRepository.Delete(new Location { Id = Id });
            return NoContent();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await _locationsRepository.GetAll(new Location(), skip, limit));
        
        [HttpGet("id/{Id}")]
        public async Task<IActionResult> GetById(string Id) => Ok(await _locationsRepository.GetById(new Location { Id = Id }));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await _locationsRepository.GetByName(new Location { Name = name }));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await _locationsRepository.SearchByName(new Location { Name = name }));

    }
}