using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Repositories;
using MycoMgmt.Domain.Models;
using MycoMgmt.Domain.Models.UserManagement;
using Newtonsoft.Json;

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
        public async Task<string> Create 
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
                CreatedOn  = DateTime.Parse(createdOn),
                CreatedBy  = createdBy
            };

            if (!string.IsNullOrEmpty(url))
                vendor.Url = url;

            if (!string.IsNullOrEmpty(notes))
                vendor.Notes = notes;
                
            var result = await _vendorRepository.Create(vendor);
            return result;
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
                CreatedOn  = DateTime.Parse(modifiedOn),
                CreatedBy  = modifiedBy
            };

            if (!string.IsNullOrEmpty(url))
                vendor.Url = url;

            if (!string.IsNullOrEmpty(notes))
                vendor.Notes = notes;

            var result = await _vendorRepository.Update(vendor, elementId);

            return Ok(result);
        }
        
        [HttpDelete("{id}")]
        public async Task<string> Delete (long id) => await _vendorRepository.Delete(id);
    }
}