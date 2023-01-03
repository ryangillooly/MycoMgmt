using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Infrastructure.Repositories;
using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.Infrastructure.Helpers;

namespace MycoMgmt.API.Controllers
{
    [Route("account")]
    [ApiController]
    public class AccountController : BaseController<AccountController>
    {
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

            var result = await Repository.CreateEntities(Logger, account);
            
            return Created("", result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update
        (
            string id,
            string name,
            string modifiedOn,
            string modifiedBy   
        )
        {
            var account = new Account
            {
                Id         = id,
                Name       = name,
                ModifiedOn = DateTime.Parse(modifiedOn),
                ModifiedBy = modifiedBy
            };

            return Ok(await Repository.Update(account));
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await Repository.Delete(new Account { Id = id });
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await Repository.GetAll(new Account(), skip, limit));

        [HttpGet("id/{Id}")]
        public async Task<IActionResult> GetById(string id) => Ok(await Repository.GetById(new Account { Id = id}));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await Repository.GetByName(new Account { Name = name}));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await Repository.SearchByName(new Account { Name = name}));
    }
}