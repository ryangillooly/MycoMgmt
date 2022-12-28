using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.Mushrooms;
using Neo4j.Driver;

namespace MycoMgmt.API.Repositories
{
    public interface ICultureRepository
    {
        Task<string> SearchByName(Culture culture);
        Task<string> GetByName(Culture culture);
        public Task<string> GetById(Culture culture);
        public Task<string> GetAll(Culture culture, int? skip, int? limit);
        public Task<string> Create(Culture culture);
        public Task Delete(Culture culture);
        public Task<string> Update(Culture culture);
    }
}