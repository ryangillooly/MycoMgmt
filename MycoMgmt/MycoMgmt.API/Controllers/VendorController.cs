using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Repositories;
using MycoMgmt.Domain.Models;

namespace MycoMgmt.API.Controllers
{
    [Route("accounts")]
    [ApiController]
    public class VendorController : Controller
    {
        private readonly IVendorRepository _vendorRepository;
        
        public VendorController(IVendorRepository repo)
        {
            _vendorRepository = repo;
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

            var results = await _vendorRepository.Create(vendor);
            return Created("", results);
        } 
        
        [HttpPut("{elementId}")]
        public async Task<IActionResult> Update
        (
            string  elementId,
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
        public async Task<IActionResult> Delete(string elementId)
        {
            await _vendorRepository.Delete(new Vendor { ElementId = elementId });
            return NoContent();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll(int? skip, int? limit) => Ok(await _vendorRepository.GetAll(new Vendor(), skip, limit));

        [HttpGet("id/{elementId}")]
        public async Task<IActionResult> GetById(string elementId) => Ok(await _vendorRepository.GetById(new Vendor { ElementId = elementId }));

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name) => Ok(await _vendorRepository.GetByName(new Vendor { Name = name }));

        [HttpGet("search/name/{name}")]
        public async Task<IActionResult> SearchByName(string name) => Ok(await _vendorRepository.SearchByName(new Vendor { Name = name }));

    }
}