using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.Infrastructure.Helpers;
using MycoMgmt.Infrastructure.Repositories;
using Neo4j.Driver;

namespace MycoMgmt.API.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserController : BaseController<UserController>
    {
        [HttpPost]
        public async Task<IActionResult> Create
        (
            string  name,
            string  account,
            string? roles,
            string? permissions,
            string  createdOn,
            string  createdBy
        )
        {
            var user = new User()
            {
                Name        = name,
                Account     = account,
                Permissions = permissions?.Split(',').ToList(),
                Roles       = roles?.Split(',').ToList(),
                CreatedOn   = DateTime.Parse(createdOn),
                CreatedBy   = createdBy
            };

            var result  = await Repository.CreateEntities(Logger, user);

            return Created("", result);
        }
        
        [HttpPut("{Id}")]
        public async Task<IActionResult> Update
        (
            string? name,
            string? account,
            string? roles,
            string? permissions,
            string  modifiedOn,
            string  modifiedBy
        )
        {
            var user = new User()
            {
                Name        = name,
                Account     = account,
                Permissions = permissions?.Split(',').ToList(),
                Roles       = roles?.Split(',').ToList(),
                ModifiedOn  = DateTime.Parse(modifiedOn),
                ModifiedBy  = modifiedBy
            };

            return Ok(await Repository.Update(user));
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await Repository.Delete(new User { Id = id });
            return NoContent();
        }
    
        [HttpGet]
        public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await Repository.GetAll(new User(), skip, limit));

        [HttpGet("id/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id) => Ok(await Repository.GetById(new User { Id = id }));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await Repository.GetByName(new User { Name = name }));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await Repository.SearchByName(new User { Name = name }));
    }
}