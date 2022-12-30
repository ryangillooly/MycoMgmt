using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Repositories;
using MycoMgmt.Domain.Models;
using Neo4j.Driver;

namespace MycoMgmt.API.Controllers
{
    [Route("strain")]
    [ApiController]
    public class StrainsController : Controller
    {
        private readonly IStrainsRepository _strainsRepository;
        
        public StrainsController(IStrainsRepository repo)
        {
            _strainsRepository = repo;
        }
        
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
            
            return Created("", await _strainsRepository.Create(strain));
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
            
            return Created("", await _strainsRepository.Update(strain));
        }
        
        [HttpDelete("{elementId}")]
        public async Task<IActionResult> Delete(string elementId)
        {
            await _strainsRepository.Delete(new Strain { ElementId = elementId });
            return NoContent();
        }
    
        [HttpGet]
        public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await _strainsRepository.GetAll(new Strain(), skip, limit));

        [HttpGet("id/{elementId}")]
        public async Task<IActionResult> GetById(string elementId) => Ok(await _strainsRepository.GetById(new Strain { ElementId = elementId }));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await _strainsRepository.GetByName(new Strain { Name = name }));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await _strainsRepository.SearchByName(new Strain { Name = name }));
    }
}