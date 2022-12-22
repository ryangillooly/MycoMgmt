using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Repositories;
using MycoMgmt.Domain.Models.UserManagement;
using Newtonsoft.Json;

namespace MycoMgmt.API.Controllers
{
    [Route("accounts")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        
        public AccountController(IAccountRepository repo)
        {
            _accountRepository = repo;
        }
        
        [HttpPost]
        public async Task<string> Create (string name, string createdOn, string createdBy, string? modifiedOn, string? modifiedBy)
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

            var result = await _accountRepository.CreateAsync(account);
            return result;
        }

        [HttpDelete("{id}")]
        public async Task<string> Delete (long id) => await _accountRepository.DeleteAsync(id);
        
        [HttpGet("all")]
        public async Task<string> GetAll (string name) => await _accountRepository.GetAllAsync();
    }
}