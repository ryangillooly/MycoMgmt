using System;
using System.Collections.Generic;
using System.Linq;
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
    [Route("api/roles")]
    [ApiController]
    public class RoleController : Controller
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IDriver _driver;
        
        public RoleController(IRoleRepository repo, IDriver driver)
        {
            this._roleRepository = repo;
            this._driver = driver;
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

            var result = await _roleRepository.AddRole(role);
            return result;
        }
        
        [HttpGet("all")]
        public async Task<string> GetAllRoles()
        {
            var node = await _roleRepository.GetAllRoles();
            return node is null ? null : JsonConvert.SerializeObject(node);
        }
    }
}