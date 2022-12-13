using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Models.Mushrooms;
using MycoMgmt.API.Models.User_Management;

namespace MycoMgmt.API.Repositories
{
    public interface IRoleRepository
    {
        [HttpPost]
        public Task<string> AddRole(IAMRole role);

        [HttpGet]
        Task<List<Dictionary<string, object>>> GetAllRoles();
    }
}