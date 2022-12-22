using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.API.Repositories
{
    public interface IPermissionRepository
    {
        [HttpPost]
        public Task<string> Add(Permission permission);

        [HttpPost]
        Task<string> Remove(Permission permission);
        
        [HttpGet]
        Task<List<object>> GetAll();
    }
}