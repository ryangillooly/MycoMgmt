using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.API.Repositories
{
    public interface IRoleRepository
    {
        [HttpPost]
        public Task<string> Add(IamRole role);

        [HttpGet]
        Task<List<object>> GetAll();
    }
}