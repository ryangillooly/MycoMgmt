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
            if (vendor == null || string.IsNullOrWhiteSpace(vendor.Name))
                throw new ArgumentNullException(nameof(vendor), "Vendor must not be null");

            var queryList = new List<string>
            {
                vendor.Create(),
                vendor.CreateCreatedOnRelationship(),
                vendor.CreateCreatedRelationship()
            };
                
            var result = await _neo4JDataAccess.RunTransaction(queryList);
            return result;
        }
        
        public async Task<string> Delete(long id)
        {
            var query = new List<string>{ $"MATCH (v:Vendor) WHERE ID(v) = { id } DETACH DELETE v RETURN v" };
            var vendor = await _neo4JDataAccess.RunTransaction(query);

            return JsonConvert.SerializeObject(vendor);
        }

        public async Task<string> Update(Vendor vendor, string elementId)
        {
            var query = $"MATCH (v:Vendor) WHERE elementId(v) = '{elementId}' ";

            var queryList = new List<string>
            {
                vendor.UpdateModifiedOnRelationship(elementId),
                vendor.UpdateModifiedRelationship(elementId)
            };
            
            // Update Name
            if (!string.IsNullOrEmpty(vendor.Name))
                queryList.Add(query + $"SET v.Name = '{vendor.Name}' RETURN v");
            
            // Update Notes
            if (!string.IsNullOrEmpty(vendor.Notes))
                queryList.Add(query + $"SET v.Notes = '{vendor.Notes}' RETURN v");
            
            // Update Url
            if (!string.IsNullOrEmpty(vendor.Url))
                queryList.Add(query + $"SET v.Url = '{vendor.Url}' RETURN v");
            
            var vendors = await _neo4JDataAccess.RunTransaction(queryList);
            return JsonConvert.SerializeObject(vendors);
        }
    }
}