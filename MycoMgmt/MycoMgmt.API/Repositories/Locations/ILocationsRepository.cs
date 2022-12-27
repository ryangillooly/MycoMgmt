using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models;

namespace MycoMgmt.API.Repositories
{
    public interface ILocationsRepository
    {
        Task<string> Create(Location location);

        Task<string> Update(Location location, string elementId);
        
        public Task<List<object>> GetAll();
    }
}