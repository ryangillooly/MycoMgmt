using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Infrastructure.Repositories;
using MycoMgmt.Core.Models;
using MycoMgmt.Infrastructure.Helpers;

namespace MycoMgmt.API.Controllers
{
    [Route("accounts")]
    [ApiController]
    public class VendorController : BaseController<VendorController>
    {
        [HttpPost]
        public async Task<IActionResult> Create 
        (
            [FromBody]
            string  name, 
            string? url,
            string? notes,
            string  createdBy
        )
        {
            var vendor = new Vendor()
            {
                Name       = name,
                Notes      = notes,
                Url        = url,
                CreatedOn  = DateTime.Now,
                CreatedBy  = createdBy
            };

            var result  = await Repository.CreateEntities(Logger, vendor);

            return Created("", result);
        } 
        
        [HttpPut("{name}")]
        public async Task<IActionResult> Update
        (
            [FromBody]
            string? name, 
            string? url,
            string? notes,
            string  modifiedBy
        )
        {
            var vendor = new Vendor()
            {
                Name       = name,
                Url        = url,
                Notes      = notes, 
                CreatedOn  = DateTime.Now,
                CreatedBy  = modifiedBy
            };
            
            return Ok(await Repository.Update(vendor));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await Repository.Delete(new Vendor { Id = id });
            return NoContent();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await Repository.GetAll(new Vendor(), skip, limit));

        [HttpGet("id/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id) => Ok(await Repository.GetById(new Vendor { Id = id }));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await Repository.GetByName(new Vendor { Name = name }));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await Repository.SearchByName(new Vendor { Name = name }));

    }
}