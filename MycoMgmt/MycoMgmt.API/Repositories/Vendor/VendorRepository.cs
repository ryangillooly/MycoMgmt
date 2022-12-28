using System.Text.RegularExpressions;
using MycoMgmt.API.DataStores.Neo4J;
using MycoMgmt.API.Helpers;
using MycoMgmt.Domain.Models;
using MycoMgmt.Domain.Models.UserManagement;
using Neo4j.Driver;
using Newtonsoft.Json;
#pragma warning disable CS8604

// ReSharper disable once CheckNamespace
namespace MycoMgmt.API.Repositories
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
            var queryList = new List<string?>
            {
                vendor.Create(),
                vendor.CreateCreatedOnRelationship(),
                vendor.CreateCreatedRelationship()
            };
                
            return await _neo4JDataAccess.RunTransaction(queryList);
        }
        
        public async Task Delete(Vendor vendor)
        {
            var delete = await _neo4JDataAccess.ExecuteWriteTransactionAsync<INode>(vendor.Delete());
        
            if(delete.ElementId == vendor.ElementId)
                _logger.LogInformation("Node with elementId {ElementId} was deleted successfully", vendor.ElementId);
            else
                _logger.LogWarning("Node with elementId {ElementId} was not deleted, or was not found for deletion", vendor.ElementId);
        }

        public async Task<string> Update(Vendor vendor)
        {
            var queryList = new List<string?>
            {
                vendor.UpdateName(),
                vendor.UpdateNotes(),
                vendor.UpdateUrl(),
                vendor.UpdateModifiedOnRelationship(),
                vendor.UpdateModifiedRelationship()
            };
            
            var results = await _neo4JDataAccess.RunTransaction(queryList);
            return JsonConvert.SerializeObject(results);
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
    
        public async Task<string> GetAll(Vendor vendor)
        {
            var result = await _neo4JDataAccess.ExecuteReadListAsync(vendor.GetAll(), "x");
            return JsonConvert.SerializeObject(result);
        }
    }
}