using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Infrastructure.Repositories;
using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.Infrastructure.Helpers;

namespace MycoMgmt.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : BaseController<AccountController>
    {
        [HttpPost]
        public async Task<IActionResult> Create ([FromBody] string name, string createdBy)
        {
            var account = new Account()
            {
                Name      = name,
                CreatedOn = DateTime.Now,
                CreatedBy = createdBy
            };

            var result = await Repository.CreateEntities(Logger, account);
            
            return Created("", result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromBody] string name, string modifiedBy, Guid id)
        {
            var account = new Account
            {
                Id         = id,
                Name       = name,
                ModifiedOn = DateTime.Now,
                ModifiedBy = modifiedBy
            };

            return Ok(await Repository.Update(account));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await Repository.Delete(new Account { Id = id });
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await Repository.GetAll(new Account(), skip, limit));

        [HttpGet("id/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id) => Ok(await Repository.GetById(new Account { Id = id}));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await Repository.GetByName(new Account { Name = name}));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await Repository.SearchByName(new Account { Name = name}));
    }
}