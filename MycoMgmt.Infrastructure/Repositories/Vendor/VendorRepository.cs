using Microsoft.Extensions.Logging;
using MycoMgmt.Domain.Models;
using MycoMgmt.Infrastructure.DataStores.Neo4J;
using Neo4j.Driver;
using Newtonsoft.Json;
#pragma warning disable CS8604

// ReSharper disable once CheckNamespace
namespace MycoMgmt.Infrastructure.Repositories
{
    public class VendorRepository : IVendorRepository
    {
        private readonly INeo4JDataAccess _neo4JDataAccess;
        private ILogger<VendorRepository> _logger;
        
        public VendorRepository(INeo4JDataAccess neo4JDataAccess, ILogger<VendorRepository> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }
        
        public async Task<string> Create(Vendor vendor)
        {
            var queryList = vendor.CreateQueryList();
            return await _neo4JDataAccess.RunTransaction(queryList);
        }
        
        public async Task<string> Update(Vendor vendor)
        {
            var queryList = vendor.UpdateQueryList();
            var results = await _neo4JDataAccess.RunTransaction(queryList);
            return JsonConvert.SerializeObject(results);
        }
        
        public async Task Delete(Vendor vendor)
        {
            var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(vendor.Delete());
        
            if(delete.ElementId == vendor.ElementId)
                _logger.LogInformation("Node with elementId {ElementId} was deleted successfully", vendor.ElementId);
            else
                _logger.LogWarning("Node with elementId {ElementId} was not deleted, or was not found for deletion", vendor.ElementId);
        }

        public async Task<string> SearchByName(Vendor vendor)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(vendor.SearchByNameQuery(), "x");
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> GetByName(Vendor vendor)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(vendor.GetByNameQuery(), "x");

            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> GetById(Vendor vendor)
        {
            var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(vendor.GetByIdQuery());
            return JsonConvert.SerializeObject(result);
        }
    
        public async Task<string> GetAll(Vendor vendor, int skip, int limit)
        {

            
            var result = await _neo4JDataAccess.ExecuteReadListAsync(vendor.GetAllQuery(skip, limit), "x");
            return JsonConvert.SerializeObject(result);
        }
    }
}