using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.API.Repositories;
using Neo4j.Driver;
using Newtonsoft.Json;

namespace MycoMgmt.API.Controllers
{
    [Route("api/permissions")]
    [ApiController]
    public class PermissionController : Controller
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IDriver _driver;
        
        public PermissionController(IPermissionRepository repo, IDriver driver)
        {
            this._permissionRepository = repo;
            this._driver = driver;
        }
        
        [HttpPost("new")]
        public async Task<string> NewPermission (string name)
        {
            var permission = new Permission()
            {
                Name = name
            };
            
            var result = await _permissionRepository.Add(permission);
            return result;
        }
        
        [HttpPost("remove")]
        public async Task<string> RemovePermission (string name)
        {
            var permission = new Permission() { Name = name };
            
            var result = await _permissionRepository.Remove(permission);
            return result;
        }
        
        [HttpGet("all")]
        public async Task<string> GetAllPermissions()
        {
            var node = await _permissionRepository.GetAll();
            return node is null ? null : JsonConvert.SerializeObject(node);
        }
    }
}