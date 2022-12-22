using System;

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

        public CultureController(ICultureRepository repo)
        {
            _cultureRepository = repo;
        }

        [HttpPost]
        public async Task<string> Create
        (
            string  name,
            string  type,
            string  strain,
            string? recipe,
            string? location,
            string? parent,
            string? child,
            string? vendor,
            bool?   successful,
            bool    finished,
            string  createdOn,
            string  createdBy,
            string? modifiedOn,
            string? modifiedBy
        )
        {
            var culture = new Culture()
            {
                Name      = name,
                Type      = type,
                Strain    = strain,
                Finished  = finished,
                CreatedOn = DateTime.Parse(createdOn),
                CreatedBy = createdBy
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

            if (modifiedBy != null)
                culture.ModifiedBy = modifiedBy;

            var result = await _cultureRepository.Create(culture);
            return result;
        }

        [HttpGet("{id:long}")]
        public async Task<string> GetById(long id) => await _cultureRepository.GetById(id.ToString());

        [HttpGet("{name}")]
        public async Task<string> GetByName(string name) => await _cultureRepository.GetByName(name);

        [HttpGet("search/name/{name}")]
        public async Task<string> SearchByName(string name) => await _cultureRepository.SearchByName(name);

        [HttpGet]
        public async Task<string> GetAll() => await _cultureRepository.GetAll();

        /*
         [HttpDelete("{id:long}")]
        public async Task<string> DeleteById(long id) => await _cultureRepository.DeleteById(id);
        
        [HttpDelete("{name}")]
        public async Task<string> DeleteByName(string name) => await _cultureRepository.DeleteByName(name);
        */
    }
}