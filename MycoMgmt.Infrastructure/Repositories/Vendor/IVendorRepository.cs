using MycoMgmt.Domain.Models;

namespace MycoMgmt.Infrastructure.Repositories
{
    public interface IVendorRepository
    {
        public Task<string> Create(Vendor vendor);
        public Task Delete(Vendor vendor);
        public Task<string> Update(Vendor vendor);
        Task<string> SearchByName(Vendor vendor);
        Task<string> GetByName(Vendor vendor);
        public Task<string> GetById(Vendor vendor);
        public Task<string> GetAll(Vendor vendor, int skip, int limit);
    }
}