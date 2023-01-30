using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Filters;
using MycoMgmt.Core.Models;
using MycoMgmt.Infrastructure.Helpers;

namespace MycoMgmt.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LocationController : BaseController<LocationController>
    {
        [HttpPost]
        public async Task<IActionResult> Create (string name, bool? agentConfigured, string createdBy)
        {
            var location = new Location()
            {
                Name            = name,
                AgentConfigured = agentConfigured,
                CreatedOn       = DateTime.Now,
                CreatedBy       = createdBy
            };
            
            var url = HttpContext.Request.GetDisplayUrl();
            var result = await ActionService.Create(location, url);

            return Created($"", result);
        }
        
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update
        (
            Guid    id,
            string? name,
            bool?   agentConfigured,
            string  modifiedBy
        )
        {
            var location = new Location
            {
                Id              = id,
                Name            = name,
                AgentConfigured = agentConfigured,
                ModifiedOn      = DateTime.Now,
                ModifiedBy      = modifiedBy
            };
            
            return Ok(await Repository.Update(location));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await Repository.Delete(new Location { Id = id });
            return NoContent();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await Repository.GetAll(new Location(), skip, limit));
        
        [HttpGet("id/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id) => Ok(await Repository.GetById(new Location { Id = id }));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await Repository.GetByName(new Location { Name = name }));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await Repository.SearchByName(new Location { Name = name }));

    }
}