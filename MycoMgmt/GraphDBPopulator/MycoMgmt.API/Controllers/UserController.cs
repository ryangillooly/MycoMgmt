using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Helpers;
using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.API.Repositories;
using Neo4j.Driver;
using Newtonsoft.Json;

namespace MycoMgmt.API.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        
        public UserController(IUserRepository repo, IDriver driver)
        {
            _userRepository = repo;
        }
        
        [HttpPost("new")]
        public async Task<string> NewUser
        (
            string  name,
            string  account,
            string? roles,
            string? permissions,
            string  createdOn,
            string  createdBy,
            string? modifiedOn,
            string? modifiedBy
        )
        {
            var user = new User()
            {
                Name      = name,
                Account   = account, 
                CreatedOn = DateTime.Parse(createdOn),
                CreatedBy = createdBy
            };
            
            user.ValidateInput(roles, permissions, modifiedOn, modifiedBy);

            var result = await _userRepository.Add(user);
            return result;
        }

        [HttpGet("all")]
        public async Task<string> GetAllUsers()
        {
            var node = await _userRepository.GetAll();
            return node is null ? null : JsonConvert.SerializeObject(node);
        }
    }
}