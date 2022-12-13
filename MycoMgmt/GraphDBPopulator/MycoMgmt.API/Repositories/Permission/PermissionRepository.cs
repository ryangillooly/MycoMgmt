using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MycoMgmt.API.DataStores.Neo4J;
using MycoMgmt.API.Models.Mushrooms;
using MycoMgmt.API.Models.User_Management;
using Neo4j.Driver;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace MycoMgmt.API.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly INeo4JDataAccess _neo4JDataAccess;
        private ILogger<PermissionRepository> _logger;
        
        public PermissionRepository(INeo4JDataAccess neo4JDataAccess, ILogger<PermissionRepository> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }
        
        public async Task<string> AddPermission(Permission permission)
        {
            if (permission == null || string.IsNullOrWhiteSpace(permission.Name))
                throw new ArgumentNullException(nameof(permission), "Permission must not be null");

            try
            {
                var queryList = new List<string>
                {
                    $@"
                        MERGE 
                        (
                            p:Permission
                            {{
                                Name: '{permission.Name}'
                            }}        
                        ) 
                        RETURN p;
                    "
                };

                var result = await _neo4JDataAccess.RunTransaction(queryList);

                return result;
            }
            catch (ClientException ex)
            {
                if (!Regex.IsMatch(ex.Message, @"Node\(\d+\) already exists with *"))
                    throw;
                
                return JsonConvert.SerializeObject(new { Message = $"A Permission already exists with the name { permission.Name }" });
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        
        public async Task<List<Dictionary<string, object>>> GetAllPermissions()
        {
            const string query = @"MATCH (p:Permission) RETURN p { Name: p.Name } ORDER BY p.Name";
            var users = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "p");

            return users;
        }
    }
}