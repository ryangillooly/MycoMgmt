using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.API.Repositories
{
    public interface IUserRepository
    {
        Task<string> SearchByName(User user);
        Task<string> GetByName(User user);
        public Task<string> GetById(User user);
        public Task<string> GetAll(User user, int? skip, int? limit);
        public Task<string> Create(User user);
        public Task Delete(User culture);
        public Task<string> Update(User user);
    }
}