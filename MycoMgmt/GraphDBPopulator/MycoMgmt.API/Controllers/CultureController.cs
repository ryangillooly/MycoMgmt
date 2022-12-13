using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MycoMgmt.API.Models;
using MycoMgmt.API.Models.Mushrooms;
using MycoMgmt.API.Repositories;
using Neo4j.Driver;
using Newtonsoft.Json;

namespace MycoMgmt.API.Controllers
{
    [Route("api/cultures")]
    [ApiController]
    public class CultureController : Controller
    {
        private readonly ICultureRepository _cultureRepository;
        private readonly IDriver _driver;
        
        public CultureController(ICultureRepository repo, IDriver driver)
        {
            this._cultureRepository = repo;
            this._driver = driver;
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
                culture.Successful = successful;

            if (modifiedOn != null)
                culture.ModifiedOn = DateTime.Parse(modifiedOn);
            
            if(modifiedBy != null)
                culture.ModifiedBy = modifiedBy;

            var result = await _cultureRepository.AddCulture(culture);
            return result;
        }

        [HttpGet("{id:long}")]
        public async Task<string> GetCultureById(long id)
        {
            var node = await _cultureRepository.GetCultureById(id.ToString());
            return node is null ? null : JsonConvert.SerializeObject(node);
        }
        
        [HttpGet("{name}")]
        public async Task<string> GetCultureByName(string name)
        {
            var node = await _cultureRepository.GetCultureByName(name);
            return node is null ? null : JsonConvert.SerializeObject(node);
        }
        
        [HttpGet("search/name/{name}")]
        public async Task<string> SearchCulturesByName(string name)
        {
            var node = await _cultureRepository.SearchCulturesByName(name);
            return node is null ? null : JsonConvert.SerializeObject(node);
        }

        [HttpGet("count")]
        public async Task<long> GetRecipeCount()
        {
            var cultureCount = await _cultureRepository.GetCultureCount();
            return cultureCount;
        }
    }
}