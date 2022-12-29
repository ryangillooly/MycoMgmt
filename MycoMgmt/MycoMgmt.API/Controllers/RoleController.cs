using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.API.Repositories;
using Neo4j.Driver;

namespace MycoMgmt.API.Controllers
{
    [Route("role")]
    [ApiController]
    public class RoleController : Controller
    {
        private readonly IRoleRepository _roleRepository;
        
        public RoleController(IRoleRepository repo)
        {
            _roleRepository = repo;
        }
        
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

            return Ok(await _roleRepository.Create(role));
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

            return Ok(await _roleRepository.Update(role));
        }
        
        [HttpDelete("{elementId}")]
        public async Task<IActionResult> Delete(string elementId)
        {
            await _roleRepository.Delete(new IamRole() { ElementId = elementId });
            return NoContent();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await _roleRepository.GetAll(new IamRole(), skip, limit));
        [HttpGet("id/{elementId}")]
        public async Task<IActionResult> GetById(string elementId) => Ok(await _roleRepository.GetById(new IamRole { ElementId = elementId }));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await _roleRepository.GetByName(new IamRole { Name = name }));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await _roleRepository.SearchByName(new IamRole { Name = name }));
    }
}