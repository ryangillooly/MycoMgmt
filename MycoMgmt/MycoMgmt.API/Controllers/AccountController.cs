using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Repositories;
using MycoMgmt.Domain.Models.UserManagement;

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

            return Created("", await _accountRepository.Create(account));
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
                ElementId  = elementId,
                Name       = name,
                ModifiedOn = DateTime.Parse(modifiedOn),
                ModifiedBy = modifiedBy
            };

            return Ok(await _accountRepository.Update(account));
        }

        [HttpDelete("{elementId}")]
        public async Task<IActionResult> Delete(string elementId)
        {
            await _accountRepository.Delete(new Account { ElementId = elementId });
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await _accountRepository.GetAll(new Account(), skip, limit));

        [HttpGet("id/{elementId}")]
        public async Task<IActionResult> GetById(string elementId) => Ok(await _accountRepository.GetById(new Account { ElementId = elementId}));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await _accountRepository.GetByName(new Account { Name = name}));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await _accountRepository.SearchByName(new Account { Name = name}));
    }
}