using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.Infrastructure.Helpers;
using MycoMgmt.Infrastructure.Repositories;
using Neo4j.Driver;
using ILogger = Neo4j.Driver.ILogger;

namespace MycoMgmt.API.Controllers
{
    [Route("role")]
    [ApiController]
    public class RoleController : BaseController<RoleController>
    {
        [HttpPost]
        public async Task<IActionResult> Create
        (
            string name, 
            string permissions,
            string createdBy,
            string createdOn
        )
        {
            var permissionList = permissions.Split(',').ToList();
            var role = new IamRole()
            {
                Name        = name,
                Permissions = permissionList,
                CreatedBy   = createdBy,
                CreatedOn   = DateTime.Parse(createdOn)
            };

            var result  = await Repository.CreateEntities(Logger, role);

            return Created("", result);
        }
        
        [HttpPut]
        public async Task<IActionResult> Update
        (
            string? name, 
            string? permissions,
            string  modifiedBy,
            string  modifiedOn
            
        )
        {
            var role = new IamRole()
            {
                Name        = name,
                Permissions = permissions?.Split(',').ToList(),
                ModifiedBy  = modifiedBy,
                ModifiedOn  = DateTime.Parse(modifiedOn)
            };

            return Ok(await Repository.Update(role));
        }
        
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(string Id)
        {
            await Repository.Delete(new IamRole() { Id = Id });
            return NoContent();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await Repository.GetAll(new IamRole(), skip, limit));
        [HttpGet("id/{Id}")]
        public async Task<IActionResult> GetById(string Id) => Ok(await Repository.GetById(new IamRole { Id = Id }));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await Repository.GetByName(new IamRole { Name = name }));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await Repository.SearchByName(new IamRole { Name = name }));
    }
}