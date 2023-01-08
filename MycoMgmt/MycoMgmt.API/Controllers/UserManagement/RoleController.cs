using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.Infrastructure.Helpers;
using MycoMgmt.Infrastructure.Repositories;
using Neo4j.Driver;
using ILogger = Neo4j.Driver.ILogger;

namespace MycoMgmt.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RoleController : BaseController<RoleController>
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string name, string permissions, string createdBy)
        {
            var role = new IamRole()
            {
                Name        = name,
                Permissions = permissions.Split(',').ToList(),
                CreatedBy   = createdBy,
                CreatedOn   = DateTime.Now
            };

            var result  = await Repository.CreateEntities(Logger, role);

            return Created("", result);
        }
        
        [HttpPut("{name}")]
        public async Task<IActionResult> Update ([FromBody] string name, string? permissions, string  modifiedBy)
        {
            var role = new IamRole()
            {
                Name        = name,
                Permissions = permissions?.Split(',').ToList(),
                ModifiedBy  = modifiedBy,
                ModifiedOn  = DateTime.Now
            };

            return Ok(await Repository.Update(role));
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await Repository.Delete(new IamRole() { Id = id });
            return NoContent();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await Repository.GetAll(new IamRole(), skip, limit));
        [HttpGet("id/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id) => Ok(await Repository.GetById(new IamRole { Id = id }));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await Repository.GetByName(new IamRole { Name = name }));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await Repository.SearchByName(new IamRole { Name = name }));
    }
}