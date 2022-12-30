using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.API.Repositories
{
    public interface IPermissionRepository
    {
        public Task<string> Create(Permission permission);
        public Task<string> Update(Permission permission);
        Task Delete(Permission permission);
        Task<string> SearchByName(Permission permission);
        Task<string> GetByName(Permission permission);
        public Task<string> GetById(Permission permission);
        public Task<string> GetAll(Permission permission, int skip, int limit);
    }
}