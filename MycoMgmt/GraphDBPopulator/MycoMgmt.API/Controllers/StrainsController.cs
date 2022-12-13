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
    [Route("api/strains")]
    [ApiController]
    public class StrainsController : Controller
    {
        private readonly IStrainsRepository _strainsRepository;
        private readonly IDriver _driver;
        
        public StrainsController(IStrainsRepository repo, IDriver driver)
        {
            this._strainsRepository = repo;
            this._driver = driver;
        }
        
        [HttpPost("new")]
        public async Task<string> NewStrain
        (
            string name, 
            string? effects,
            string? createdOn,
            string? createdBy,
            string? modifiedOn,
            string? modifiedBy
        )
        {
            var strain = new Strain() { Name = name };

            if (effects != null)
                strain.Effects = effects;

            if (createdOn != null)
                strain.CreatedOn = DateTime.Parse(createdOn);
            
            if(createdBy != null)
                strain.CreatedBy = createdBy;
            
            if (modifiedOn != null)
                strain.ModifiedOn = DateTime.Parse(modifiedOn);
            
            if(modifiedBy != null)
                strain.ModifiedBy = modifiedBy;

            var result = await _strainsRepository.AddStrain(strain);
            return result;
        }
        
        [HttpGet("all")]
        public async Task<string> GetAllLocations(string name)
        {
            var node = await _strainsRepository.GetAllStrains();
            return node is null ? null : JsonConvert.SerializeObject(node);
        }
    }
}