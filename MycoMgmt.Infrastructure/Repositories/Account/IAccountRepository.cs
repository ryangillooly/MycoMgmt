using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.Infrastructure.Repositories
{
    public interface IAccountRepository
    {
        Task<string> SearchByName(Account account);
        Task<string> GetByName(Account account);
        public Task<string> GetById(Account account);
        public Task<string> GetAll(Account account, int skip, int limit);
        public Task<string> Create(Account account);
        public Task<string> Delete(Account account);
        public Task<string> Update(Account account);
        
    }
}