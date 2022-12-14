using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models;

namespace MycoMgmt.API.Repositories
{
    public interface IStrainsRepository
    {
        [HttpGet]
        public Task<List<Dictionary<string, object>>> GetAll();

        [HttpPost]
        Task<string> Add(Strain strain);
    }
}