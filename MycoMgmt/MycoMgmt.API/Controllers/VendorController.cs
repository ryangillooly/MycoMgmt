using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Infrastructure.Repositories;
using MycoMgmt.Domain.Models;
using MycoMgmt.Infrastructure.Helpers;

namespace MycoMgmt.API.Controllers
{
    [Route("accounts")]
    [ApiController]
    public class VendorController : Controller
    {
        private readonly ActionRepository _vendorRepository;
        private readonly ILogger<VendorController> _logger;

        public VendorController(ActionRepository repo, ILogger<VendorController> logger)
        {
            _vendorRepository = repo;
            _logger = logger;
        }
        
        [HttpPost]
        public async Task<IActionResult> Create 
        (
            string  name, 
            string? url,
            string? notes,
            string  createdOn, 
            string  createdBy
        )
        {
            var vendor = new Vendor()
            {
                Name       = name,
                Notes      = notes,
                Url        = url,
                CreatedOn  = DateTime.Parse(createdOn),
                CreatedBy  = createdBy
            };

            var result  = await _vendorRepository.CreateEntities(_logger, vendor);

            return Created("", result);
        } 
        
        [HttpPut("{Id}")]
        public async Task<IActionResult> Update
        (
            string  Id,
            string? name, 
            string? url,
            string? notes,
            string  modifiedOn, 
            string  modifiedBy
        )
        {
            var vendor = new Vendor()
            {
                Name       = name,
                Url        = url,
                Notes      = notes, 
                CreatedOn  = DateTime.Parse(modifiedOn),
                CreatedBy  = modifiedBy
            };
            
            return Ok(await _vendorRepository.Update(vendor));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string Id)
        {
            await _vendorRepository.Delete(new Vendor { Id = Id });
            return NoContent();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await _vendorRepository.GetAll(new Vendor(), skip, limit));

        [HttpGet("id/{Id}")]
        public async Task<IActionResult> GetById(string Id) => Ok(await _vendorRepository.GetById(new Vendor { Id = Id }));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await _vendorRepository.GetByName(new Vendor { Name = name }));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await _vendorRepository.SearchByName(new Vendor { Name = name }));

    }
}