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
    public class RoleController : Controller
    {
        private readonly ActionRepository _roleRepository;
        private readonly ILogger<RoleController> _logger;
        
        public RoleController(ActionRepository repo, ILogger<RoleController> logger)
        {
            _roleRepository = repo;
            _logger = logger;
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

            var result  = await _roleRepository.CreateEntities(_logger, role);

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

            return Ok(await _roleRepository.Update(role));
        }
        
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(string Id)
        {
            await _roleRepository.Delete(new IamRole() { Id = Id });
            return NoContent();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await _roleRepository.GetAll(new IamRole(), skip, limit));
        [HttpGet("id/{Id}")]
        public async Task<IActionResult> GetById(string Id) => Ok(await _roleRepository.GetById(new IamRole { Id = Id }));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await _roleRepository.GetByName(new IamRole { Name = name }));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await _roleRepository.SearchByName(new IamRole { Name = name }));
    }
}