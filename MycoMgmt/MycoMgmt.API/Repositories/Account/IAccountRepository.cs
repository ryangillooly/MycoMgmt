using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.API.Repositories
{
    public interface IAccountRepository
    {
        public Task<string> CreateAsync(Account account);
        
        public Task<string> DeleteAsync(long id);
        
        public Task<string> UpdateAsync(Account account);
        
        public Task<string> GetAllAsync();
    }
}