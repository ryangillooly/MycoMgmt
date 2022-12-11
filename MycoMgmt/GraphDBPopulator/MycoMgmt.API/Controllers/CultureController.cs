using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MycoMgmt.API.Models;
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
        public async void NewCulture
        (
            string? name, 
            string? type, 
            string? strain, 
            string? recipe,
            string? location,
            string? parent,
            string? child,
            string? vendor,
            bool? successful,
            bool finished,
            string created,
            string? user
        )
        {

            Enum.TryParse(strain, out Strain strains);
            Enum.TryParse(type, out CultureTypes cultureType);
            Enum.TryParse(location, out Locations locations);

            var culture = new Culture()
            {
                Name   = name,
                Type   = cultureType,
                Strain = strains,
                Recipe = recipe,
                Location = locations,
                Parent = parent ,
                Child = child,
                Vendor = vendor,
                Successful = successful,
                Finished = finished,
                CreatedOn = DateTime.Parse(created),
                CreatedBy = user
            };

            await _cultureRepository.AddCulture(culture);
        }

        [HttpGet("{id}")]
        public async Task<string> GetCultureById(string id)
        {
            var node = await _cultureRepository.SearchCultureById(id);
            return node is null ? null : JsonConvert.SerializeObject(node);
        }
        
        [HttpGet("count")]
        public async Task<long> GetRecipeCount()
        {
            var recipeCount = await _cultureRepository.GetCultureCount();
            Console.WriteLine($"RecipeCount - { recipeCount.ToString() }");
            return recipeCount;
        }
    }
}