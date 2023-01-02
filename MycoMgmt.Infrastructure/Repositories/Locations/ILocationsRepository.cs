using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models;

namespace MycoMgmt.Infrastructure.Repositories
{
    public interface ILocationsRepository
    {
        Task<string> Create(Location location);

        Task<string> Update(Location location);
        Task Delete(Location location);
        
        Task<string> GetAll(Location location, int skip, int limit);
        Task<string> SearchByName(Location location);
        Task<string> GetByName(Location location);
        Task<string> GetById(Location location);
    }
}