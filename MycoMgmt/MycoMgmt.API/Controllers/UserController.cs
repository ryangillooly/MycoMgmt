using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.API.Repositories;
using Neo4j.Driver;

namespace MycoMgmt.API.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        
        public UserController(IUserRepository repo)
        {
            _userRepository = repo;
        }
        
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

            return Created("", await _userRepository.Create(user));
        }
        
        [HttpPut("{elementId}")]
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

            return Ok(await _userRepository.Update(user));
        }
        
        [HttpDelete("{elementId}")]
        public async Task<IActionResult> Delete(string elementId)
        {
            await _userRepository.Delete(new User { ElementId = elementId });
            return NoContent();
        }
    
        [HttpGet]
        public async Task<IActionResult> GetAll(int? skip, int? limit) => Ok(await _userRepository.GetAll(new User(), skip, limit));

        [HttpGet("id/{elementId}")]
        public async Task<IActionResult> GetById(string elementId) => Ok(await _userRepository.GetById(new User { ElementId = elementId }));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await _userRepository.GetByName(new User { Name = name }));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await _userRepository.SearchByName(new User { Name = name }));
    }
}