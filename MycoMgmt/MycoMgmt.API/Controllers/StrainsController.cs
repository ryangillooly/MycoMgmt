using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Infrastructure.Repositories;
using MycoMgmt.Domain.Models;
using MycoMgmt.Infrastructure.Helpers;
using Neo4j.Driver;

namespace MycoMgmt.API.Controllers
{
    [Route("strain")]
    [ApiController]
    public class StrainsController : BaseController<StrainsController>
    {
        [HttpPost]
        public async Task<IActionResult> Create
        (
            string name, 
            string? effects,
            string  createdOn,
            string  createdBy
        )
        {
            var strain = new Strain
            {
                Name      = name,
                Effects   = effects,
                CreatedBy = createdBy,
                CreatedOn = DateTime.Parse(createdOn)
            };
            
            var result  = await Repository.CreateEntities(Logger, strain);

            return Created("", result);
        }
        
        [HttpPut]
        public async Task<IActionResult> Update
        (
            string? name, 
            string? effects,
            string  modifiedOn,
            string  modifiedBy
        )
        {
            var strain = new Strain
            {
                Name       = name,
                Effects    = effects,
                ModifiedBy = modifiedBy,
                ModifiedOn = DateTime.Parse(modifiedOn)
            };
            
            return Created("", await Repository.Update(strain));
        }
        
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(string Id)
        {
            await Repository.Delete(new Strain { Id = Id });
            return NoContent();
        }
    
        [HttpGet]
        public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await Repository.GetAll(new Strain(), skip, limit));

        [HttpGet("id/{Id}")]
        public async Task<IActionResult> GetById(string Id) => Ok(await Repository.GetById(new Strain { Id = Id }));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await Repository.GetByName(new Strain { Name = name }));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await Repository.SearchByName(new Strain { Name = name }));
    }
}