using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MycoMgmt.API.Models;
using MycoMgmt.API.Repositories;
using Neo4j.Driver;

namespace MycoMgmt.API.Controllers
{
    [Route("api/cultures")]
    [ApiController]
    public class CultureController : Controller
    {
        private readonly ICultureRepository _repo;
        private readonly IDriver _driver;
        
        public CultureController(ICultureRepository repo, IDriver driver)
        {
            this._repo = repo;
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
            string? modified,
            string? user
        )
        {           
            Enum.TryParse(strain, out Strain strains);
            Enum.TryParse(type, out CultureTypes cultureType);
            Enum.TryParse(location, out Locations locations);
            
            var culture = new Culture()
            {
                Id     = Guid.NewGuid().ToString(),
                Name   = name,
                Type   = cultureType,
                Strain = strains,
                Recipe = recipe,
                Location = locations,
                Parent = parent ,
                Child = child,
                Vendor = vendor,
                Successful = (bool)successful,
                Finished = finished,
                Created = new Created { ByUserId = user, On = DateTime.Parse(created) },
                Modified = new Modified { ByUserId = user, On = DateTime.Parse(modified) }
            };

            await _repo.AddCulture(culture);
        }

        [HttpGet("count")]
        public async Task<long> GetRecipeCount()
        {
            var recipeCount = await _repo.GetCultureCount();
            Console.WriteLine($"RecipeCount - { recipeCount.ToString() }");
            return recipeCount;
        }
    }
}