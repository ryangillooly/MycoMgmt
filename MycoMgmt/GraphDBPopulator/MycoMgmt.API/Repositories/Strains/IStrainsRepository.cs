using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MycoMgmt.API.Repositories
{
    public interface IStrainsRepository
    {
        [HttpPost]
        public Task<List<Dictionary<string, object>>> GetAllStrains();
    }
}