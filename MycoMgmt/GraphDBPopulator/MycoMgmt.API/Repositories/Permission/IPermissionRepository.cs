using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Models.Mushrooms;
using MycoMgmt.API.Models.User_Management;

namespace MycoMgmt.API.Repositories
{
    public interface IPermissionRepository
    {
        [HttpPost]
        public Task<string> AddPermission(Permission permission);

        [HttpPost]
        Task<string> RemovePermission(Permission permission);
        
        [HttpGet]
        Task<List<Dictionary<string, object>>> GetAllPermissions();
    }
}