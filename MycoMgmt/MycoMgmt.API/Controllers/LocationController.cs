using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Helpers;
using MycoMgmt.Core.Contracts;
using MycoMgmt.Core.Models;

namespace MycoMgmt.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LocationController : BaseController<LocationController>
    {
        [HttpPost]
        public async Task<IActionResult> Create ([FromBody] CreateLocationRequest request) =>
            Created($"", await request.Create<Location>(Mapper, ActionService, HttpContext.Request.GetDisplayUrl()));
        
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update ([FromBody] UpdateLocationRequest request, Guid id) =>
            Ok(await request.Update<Location>(Mapper, ActionService, HttpContext.Request.GetDisplayUrl(), id));

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            ActionService.Delete(new Location { Id = id });
            return NoContent();
        }
    
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id) => 
            Ok(await ActionService.GetById(new Location { Id = id }));

        [HttpGet]
        public async Task<IActionResult> GetAll(int skip = 0, int limit = 20) => 
            Ok(await ActionService.GetAll(new Location(), skip, limit));
    
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => 
            Ok(await ActionService.GetByName(new Location { Name = name }));
   
        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name, int skip = 0, int limit = 20) => 
            Ok(await ActionService.SearchByName(new Location { Name = name }, skip, limit));

    }
}