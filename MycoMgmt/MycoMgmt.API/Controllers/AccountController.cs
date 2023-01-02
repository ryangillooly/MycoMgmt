using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Infrastructure.Repositories;
using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.Infrastructure.Helpers;

namespace MycoMgmt.API.Controllers
{
    [Route("account")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly ActionRepository _accountRepository;
        private readonly ILogger<AccountController> _logger;
        
        public AccountController(ActionRepository repo, ILogger<AccountController> logger)
        {
            _accountRepository = repo;
            _logger = logger;
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

            var result = await _accountRepository.CreateEntities(_logger, account);
            
            return Created("", result);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update
        (
            string Id,
            string name,
            string modifiedOn,
            string modifiedBy   
        )
        {
            var account = new Account
            {
                Id  = Id,
                Name       = name,
                ModifiedOn = DateTime.Parse(modifiedOn),
                ModifiedBy = modifiedBy
            };

            return Ok(await _accountRepository.Update(account));
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(string Id)
        {
            await _accountRepository.Delete(new Account { Id = Id });
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await _accountRepository.GetAll(new Account(), skip, limit));

        [HttpGet("id/{Id}")]
        public async Task<IActionResult> GetById(string Id) => Ok(await _accountRepository.GetById(new Account { Id = Id}));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await _accountRepository.GetByName(new Account { Name = name}));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await _accountRepository.SearchByName(new Account { Name = name}));
    }
}