using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models;

namespace MycoMgmt.API.Repositories
{
    public interface IStrainsRepository
    {
        Task<string> SearchByName(Strain strain);
        Task<string> GetByName(Strain strain);
        public Task<string> GetById(Strain strain);
        public Task<string> GetAll(Strain strain, int? skip, int? limit);
        public Task<string> Create(Strain strain);
        public Task Delete(Strain strain);
        public Task<string> Update(Strain strain);
    }
}