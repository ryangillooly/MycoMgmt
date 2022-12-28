using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.Mushrooms;
using Neo4j.Driver;

namespace MycoMgmt.API.Repositories
{
    public interface IBulkRepository
    {
        Task<string> SearchByName(Bulk bulk);
        Task<string> GetByName(Bulk bulk);
        public Task<string> GetById(Bulk bulk);
        public Task<string> GetAll(Bulk bulk);
        public Task<string> Create(Bulk bulk);
        public Task Delete(Bulk bulk);
        public Task<string> Update(Bulk bulk);
    }
}