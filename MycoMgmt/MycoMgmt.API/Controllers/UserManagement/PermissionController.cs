using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.Infrastructure.Helpers;
using MycoMgmt.Infrastructure.Repositories;
using Neo4j.Driver;

namespace MycoMgmt.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PermissionController : BaseController<PermissionController>
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string name)
        {
            var permission = new Permission { Name = name };
            var result  = await Repository.CreateEntities(Logger, permission);

            return Created("", result);
        }
        
        
        [HttpPut("{name}")]
        public async Task<IActionResult> Update ([FromBody] string name, string modifiedBy)
        {
            var permission = new Permission()
            {
                Name        = name,
                ModifiedOn  = DateTime.Now,
                ModifiedBy  = modifiedBy
            };
            
            return Ok(await Repository.Update(permission));
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            await Repository.Delete(new Permission { Id = id});
            return NoContent();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await Repository.GetAll(new Permission(), skip, limit));

        [HttpGet("id/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id) => Ok(await Repository.GetById(new Permission { Id = id }));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await Repository.GetByName(new Permission { Name = name }));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await Repository.SearchByName(new Permission { Name = name }));
    }
}