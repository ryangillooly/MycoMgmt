using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Repositories;
using MycoMgmt.Domain.Models;
using Neo4j.Driver;
using Newtonsoft.Json;

namespace MycoMgmt.API.Controllers
{
    [Route("strains")]
    [ApiController]
    public class StrainsController : Controller
    {
        private readonly IStrainsRepository _strainsRepository;
        
        public StrainsController(IStrainsRepository repo, IDriver driver)
        {
            _strainsRepository = repo;
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

            var result = await _strainsRepository.Add(strain);
            return result;
        }
        
        [HttpGet("all")]
        public async Task<string> GetAllLocations()
        {
            var node = await _strainsRepository.GetAll();
            return node is null ? null : JsonConvert.SerializeObject(node);
        }
    }
}