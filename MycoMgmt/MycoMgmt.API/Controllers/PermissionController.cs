using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.Infrastructure.Repositories;
using Neo4j.Driver;

namespace MycoMgmt.API.Controllers
{
    [Route("permission")]
    [ApiController]
    public class PermissionController : Controller
    {
        private readonly IPermissionRepository _permissionRepository;
        
        public PermissionController(IPermissionRepository repo)
        {
            _permissionRepository = repo;
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            var permission = new Permission()
            {
                Name = name
            };
            
            return Created("", await _permissionRepository.Create(permission));
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
            
            return Ok(await _permissionRepository.Update(permission));
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(string elementId)
        {
            await _permissionRepository.Delete(new Permission { ElementId = elementId});
            return NoContent();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await _permissionRepository.GetAll(new Permission(), skip, limit));

        [HttpGet("id/{elementId}")]
        public async Task<IActionResult> GetById(string elementId) => Ok(await _permissionRepository.GetById(new Permission { ElementId = elementId }));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await _permissionRepository.GetByName(new Permission { Name = name }));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await _permissionRepository.SearchByName(new Permission { Name = name }));
    }
}