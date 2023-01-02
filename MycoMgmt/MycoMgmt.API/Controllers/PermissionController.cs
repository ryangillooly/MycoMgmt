using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.Infrastructure.Helpers;
using MycoMgmt.Infrastructure.Repositories;
using Neo4j.Driver;

namespace MycoMgmt.API.Controllers
{
    [Route("permission")]
    [ApiController]
    public class PermissionController : Controller
    {
        private readonly BaseRepository<Permission> _permissionRepository;
        private readonly ILogger<PermissionController> _logger;

        public PermissionController(BaseRepository<Permission> repo, ILogger<PermissionController> logger)
        {
            _permissionRepository = repo;
            _logger = logger;
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            var permission = new Permission()
            {
                Name = name
            };
            
            var result  = await _permissionRepository.CreateEntities(_logger, permission);

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
            
            return Ok(await _permissionRepository.Update(permission));
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(string Id)
        {
            await _permissionRepository.Delete(new Permission { Id = Id});
            return NoContent();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await _permissionRepository.GetAll(new Permission(), skip, limit));

        [HttpGet("id/{Id}")]
        public async Task<IActionResult> GetById(string Id) => Ok(await _permissionRepository.GetById(new Permission { Id = Id }));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await _permissionRepository.GetByName(new Permission { Name = name }));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await _permissionRepository.SearchByName(new Permission { Name = name }));
    }
}