using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Core.Models;
using MycoMgmt.Infrastructure.Helpers;

namespace MycoMgmt.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StrainController : BaseController<StrainController>
    {
        [HttpPost]
        public async Task<IActionResult> Create ([FromBody] string name, string? effects, string createdBy)
        {
            var strain = new Strain
            {
                Name      = name,
                Effects   = effects,
                CreatedBy = createdBy,
                CreatedOn = DateTime.Now
            };
            
            var result  = await Repository.CreateEntities(Logger, strain);

            //return Created("", result);
            return Created("", "");
        }
        
        [HttpPut]
        public async Task<IActionResult> Update ([FromBody] string? name, string? effects, string modifiedBy)
        {
            var strain = new Strain
            {
                Name       = name,
                Effects    = effects,
                ModifiedBy = modifiedBy,
                ModifiedOn = DateTime.Now
            };
            
            return Created("", await Repository.Update(strain));
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await Repository.Delete(new Strain { Id = id });
            return NoContent();
        }
    
        [HttpGet]
        public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await Repository.GetAll(new Strain(), skip, limit));

        [HttpGet("id/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id) => Ok(await Repository.GetById(new Strain { Id = id }));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await Repository.GetByName(new Strain { Name = name }));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name, int skip = 0, int limit = 20) => Ok(await Repository.SearchByName(new Strain { Name = name }, skip, limit));
    }
}