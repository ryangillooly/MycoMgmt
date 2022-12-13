using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MycoMgmt.API.Models;
using MycoMgmt.API.Models.Mushrooms;
using MycoMgmt.API.Models.User_Management;
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
            
            var result = await _permissionRepository.AddPermission(permission);
            return result;
        }
        
        [HttpPost("remove")]
        public async Task<string> RemovePermission (string name)
        {
            var permission = new Permission() { Name = name };
            
            var result = await _permissionRepository.RemovePermission(permission);
            return result;
        }
        
        [HttpGet("all")]
        public async Task<string> GetAllPermissions()
        {
            var node = await _permissionRepository.GetAllPermissions();
            return node is null ? null : JsonConvert.SerializeObject(node);
        }
    }
}