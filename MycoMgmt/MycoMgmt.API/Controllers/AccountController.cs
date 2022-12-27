using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Repositories;
using MycoMgmt.Domain.Models.UserManagement;
using Newtonsoft.Json;

namespace MycoMgmt.API.Controllers
{
    [Route("account")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        
        public AccountController(IAccountRepository repo)
        {
            _accountRepository = repo;
        }
        
        [HttpPost]
        public async Task<IActionResult> Create 
        (
            string  name, 
            string  createdOn, 
            string  createdBy
        )
        {
            var account = new Account()
            {
                Name      = name,
                CreatedOn = DateTime.Parse(createdOn),
                CreatedBy = createdBy
            };

            var result = await _accountRepository.Create(account);
            return Created("", result);
        }

        [HttpPut("{elementId}")]
        public async Task<IActionResult> Update
        (
            string elementId,
            string name,
            string modifiedOn,
            string modifiedBy   
        )
        {
            var account = new Account
            {
                Name       = name,
                ModifiedOn = DateTime.Parse(modifiedOn),
                ModifiedBy = modifiedBy
            };

            var result = await _accountRepository.Update(account, elementId);

            return Ok(result);
        }
        
        [HttpDelete("{elementId}")]
        public async Task<string> Delete (string elementId) => await _accountRepository.Delete(elementId);
        
        [HttpGet]
        public async Task<string> GetAll (string name) => await _accountRepository.GetAll();
    }
}