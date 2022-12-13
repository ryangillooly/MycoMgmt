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
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IDriver _driver;
        
        public AccountController(IAccountRepository repo, IDriver driver)
        {
            this._accountRepository = repo;
            this._driver = driver;
        }
        
        [HttpPost("new")]
        public async Task<string> NewAccount
        (
            string name,
            string createdOn,
            string createdBy,
            string? modifiedOn,
            string? modifiedBy
        )
        {
            var account = new Account()
            {
                Name       = name,
                CreatedOn  = DateTime.Parse(createdOn),
                CreatedBy  = createdBy
            };

            if (modifiedOn != null)
                account.ModifiedOn = DateTime.Parse(modifiedOn);
            
            if(modifiedBy != null)
                account.ModifiedBy = modifiedBy;

            var result = await _accountRepository.AddAccount(account);
            return result;
        }
        
        [HttpGet("all")]
        public async Task<string> GetAllLocations(string name)
        {
            var node = await _accountRepository.GetAllAccounts();
            return node is null ? null : JsonConvert.SerializeObject(node);
        }
    }
}