using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.API.Repositories
{
    public interface IUserRepository
    {
        [HttpPost]
        public Task<string> Add(User user);

        [HttpGet]
        Task<List<Dictionary<string, object>>> GetAll();
    }
}