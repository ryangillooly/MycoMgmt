using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.API.Repositories
{
    public interface IAccountRepository
    {
        public Task<string> Create(Account account);
        
        public Task<string> Delete(string elementId);
        
        public Task<string> Update(Account account, string elementId);
        
        public Task<string> GetAll();
    }
}