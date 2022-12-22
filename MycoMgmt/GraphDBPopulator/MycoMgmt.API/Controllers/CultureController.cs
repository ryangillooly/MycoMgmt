using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Repositories;
using MycoMgmt.Domain.Models.Mushrooms;
using Neo4j.Driver;
using Newtonsoft.Json;

namespace MycoMgmt.API.Controllers
{
    [Route("cultures")]
    [ApiController]
    public class CultureController : Controller
    {
        private readonly ICultureRepository _cultureRepository;
        
        public CultureController(ICultureRepository repo, IDriver driver)
        {
            _cultureRepository = repo;
        }

        [HttpPost("new")]
        public async Task<string> NewCulture
        (
            string name, 
            string type, 
            string strain, 
            string? recipe,
            string? location,
            string? parent,
            string? child,
            string? vendor,
            bool? successful,
            bool finished,
            string createdOn,
            string createdBy,
            string? modifiedOn,
            string? modifiedBy
        )
        {
            var culture = new Culture()
            {
                Name       = name,
                Type       = type,
                Strain     = strain,
                Finished   = finished,
                CreatedOn  = DateTime.Parse(createdOn),
                CreatedBy  = createdBy
            };

            if (recipe != null)
                culture.Recipe = recipe;
            
            if (location != null)
                culture.Location = location;
            
            if (parent != null)
                culture.Parent = parent;
            
            if (child != null)
                culture.Child = child;
            
            if (vendor != null)
                culture.Vendor = vendor;
            
            if (successful != null)
                culture.Successful = successful.Value;

            if (modifiedOn != null)
                culture.ModifiedOn = DateTime.Parse(modifiedOn);
            
            if(modifiedBy != null)
                culture.ModifiedBy = modifiedBy;

            var result = await _cultureRepository.Add(culture);
            return result;
        }

        [HttpGet("{id:long}")]
        public async Task<string> GetCultureById(long id)
        {
            var node = await _cultureRepository.GetById(id.ToString());
            return node is null ? null : JsonConvert.SerializeObject(node);
        }
        
        [HttpGet("{name}")]
        public async Task<string> GetCultureByName(string name)
        {
            var node = await _cultureRepository.GetByName(name);
            return node is null ? null : JsonConvert.SerializeObject(node);
        }
        
        [HttpGet("search/name/{name}")]
        public async Task<string> SearchCulturesByName(string name)
        {
            var node = await _cultureRepository.SearchByName(name);
            return node is null ? null : JsonConvert.SerializeObject(node);
        }
        
        [HttpGet("all")]
        public async Task<string> GetCultures()
        {
            var cultures = await _cultureRepository.GetAll();
            return cultures is null ? null : JsonConvert.SerializeObject(cultures);
        }        

        [HttpGet("count")]
        public async Task<long> GetRecipeCount()
        {
            var cultureCount = await _cultureRepository.GetCount();
            return cultureCount;
        }
        
        [HttpGet("test")]
        public async Task<string> GetNodes()
        {
            var cultureCount = await _cultureRepository.Test();
            return cultureCount;
        }
    }
}