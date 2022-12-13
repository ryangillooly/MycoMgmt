using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Models;

namespace MycoMgmt.API.Repositories
{
    public interface IStrainsRepository
    {
        [HttpGet]
        public Task<List<Dictionary<string, object>>> GetAllStrains();

        [HttpPost]
        Task<string> AddStrain(Strain strain);
    }
}