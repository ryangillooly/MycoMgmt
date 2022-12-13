﻿using System;
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
    [Route("api/users")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IDriver _driver;
        
        public UserController(IUserRepository repo, IDriver driver)
        {
            this._userRepository = repo;
            this._driver = driver;
        }
        
        [HttpPost("new")]
        public async Task<string> NewUser
        (
            string name,
            string? roles,
            string? permissions,
            string createdOn,
            string createdBy,
            string? modifiedOn,
            string? modifiedBy
        )
        {
            var user = new User()
            {
                Name       = name,
                CreatedOn  = DateTime.Parse(createdOn),
                CreatedBy  = createdBy
            };
            
            if(permissions != null)
                user.Permissions = permissions.Split(',').ToList();
            
            if(roles != null)
                user.Roles = roles.Split(',').ToList();

            if (modifiedOn != null)
                user.ModifiedOn = DateTime.Parse(modifiedOn);
            
            if(modifiedBy != null)
                user.ModifiedBy = modifiedBy;

            var result = await _userRepository.AddUser(user);
            return result;
        }
        
        [HttpGet("all")]
        public async Task<string> GetAllUsers()
        {
            var node = await _userRepository.GetAllUsers();
            return node is null ? null : JsonConvert.SerializeObject(node);
        }
    }
}