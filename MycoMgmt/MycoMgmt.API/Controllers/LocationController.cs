using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Infrastructure.Repositories;
using MycoMgmt.Domain.Models;
using MycoMgmt.Infrastructure.Helpers;
using Neo4j.Driver;

namespace MycoMgmt.API.Controllers
{
    [Route("location")]
    [ApiController]
    public class LocationController : BaseController<LocationController>
    {
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
            
            var result = await Repository.CreateEntities(Logger, location);
            
            return Created($"", result);
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
                Id              = Id,
                Name            = name,
                AgentConfigured = agentConfigured,
                ModifiedOn      = DateTime.Parse(modifiedOn),
                ModifiedBy      = modifiedBy
            };
            
            return Ok(await Repository.Update(location));
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(string Id)
        {
            await Repository.Delete(new Location { Id = Id });
            return NoContent();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await Repository.GetAll(new Location(), skip, limit));
        
        [HttpGet("id/{Id}")]
        public async Task<IActionResult> GetById(string Id) => Ok(await Repository.GetById(new Location { Id = Id }));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await Repository.GetByName(new Location { Name = name }));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await Repository.SearchByName(new Location { Name = name }));

    }
}