using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.API.Repositories;
using Neo4j.Driver;
using Newtonsoft.Json;

namespace MycoMgmt.API.Controllers
{
    [Route("roles")]
    [ApiController]
    public class RoleController : Controller
    {
        private readonly IRoleRepository _roleRepository;
        
        public RoleController(IRoleRepository repo, IDriver driver)
        {
            _roleRepository = repo;
        }
        
        [HttpPost("new")]
        public async Task<string> NewRole (string name, string permissions)
        {
            var permissionList = permissions.Split(',').ToList();
            var role = new IAMRole()
            {
                Name = name,
                Permissions = permissionList
            };

            var result = await _roleRepository.Add(role);
            return result;
        }
        
        [HttpGet("all")]
        public async Task<string> GetAllRoles()
        {
            var node = await _roleRepository.GetAll();
            return node is null ? null : JsonConvert.SerializeObject(node);
        }
    }
}