using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Models.Mushrooms;
using MycoMgmt.API.Models.User_Management;

namespace MycoMgmt.API.Repositories
{
    public interface IUserRepository
    {
        [HttpPost]
        public Task<string> AddUser(User user);

        [HttpGet]
        Task<List<Dictionary<string, object>>> GetAllUsers();
    }
}