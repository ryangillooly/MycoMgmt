using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.API.Repositories
{
    public interface IAccountRepository
    {
        [HttpPost]
        public Task<string> Add(Account account);

        [HttpGet]
        Task<List<Dictionary<string, object>>> GetAll();
    }
}