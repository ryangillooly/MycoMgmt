using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.Infrastructure.Repositories
{
    public interface IRoleRepository
    {
        Task<string> SearchByName(IamRole role);
        Task<string> GetByName(IamRole role);
        public Task<string> GetById(IamRole role);
        public Task<string> GetAll(IamRole role, int skip, int limit);
        public Task<string> Create(IamRole role);
        public Task Delete(IamRole role);
        public Task<string> Update(IamRole role);
    }
}