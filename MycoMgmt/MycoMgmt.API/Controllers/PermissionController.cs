using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.Infrastructure.Helpers;
using MycoMgmt.Infrastructure.Repositories;
using Neo4j.Driver;

namespace MycoMgmt.API.Controllers
{
    [Route("permission")]
    [ApiController]
    public class PermissionController : BaseController<PermissionController>
    {
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            var permission = new Permission()
            {
                Name = name
            };
            
            var result  = await Repository.CreateEntities(Logger, permission);

            return Created("", result);
        }
        
        
        [HttpPut("{name}")]
        public async Task<IActionResult> Update
        (
            string newName,
            string modifiedBy,
            string modifiedOn
        )
        {
            var permission = new Permission()
            {
                Name        = newName,
                ModifiedOn  = DateTime.Parse(modifiedOn),
                ModifiedBy  = modifiedBy
            };
            
            return Ok(await Repository.Update(permission));
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(string Id)
        {
            await Repository.Delete(new Permission { Id = Id});
            return NoContent();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await Repository.GetAll(new Permission(), skip, limit));

        [HttpGet("id/{Id}")]
        public async Task<IActionResult> GetById(string Id) => Ok(await Repository.GetById(new Permission { Id = Id }));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await Repository.GetByName(new Permission { Name = name }));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await Repository.SearchByName(new Permission { Name = name }));
    }
}