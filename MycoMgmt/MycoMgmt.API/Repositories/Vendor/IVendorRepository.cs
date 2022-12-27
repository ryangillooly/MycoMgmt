using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.API.Repositories
{
    public interface IVendorRepository
    {
        public Task<string> Create(Vendor vendor);
        
        public Task<string> Delete(long id);
        
        public Task<string> Update(Vendor vendor, string elementId);
    }
}