using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.API.DataStores.Neo4J;
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
        
        public async Task<string> Add(Permission permission)
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
        
        public async Task<string> Remove(Permission permission)
        {
            if (permission == null || string.IsNullOrWhiteSpace(permission.Name))
                throw new ArgumentNullException(nameof(permission), "Permission must not be null");

            return await PersistToDatabase(permission);
        }

        private async Task<string> PersistToDatabase(Permission permission)
        {
            try
            {
                var queryList = CreateQueryList(permission);
                var result = await _neo4JDataAccess.RunTransaction(queryList);
                return result;
            }
            catch (ClientException ex)
            {
                return JsonConvert.SerializeObject(new { Message = $"Permission error.... {ex}" });
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        private static List<string> CreateQueryList(Permission permission)
        {
            var queryList = new List<string>
            {
                $@"
                        MATCH (p:Permission {{ Name: '{permission.Name}' }}) 
                        DETACH DELETE p
                        RETURN p;
                    "
            };
            return queryList;
        }


        public async Task<List<object>> GetAll()
        {
            const string query = @"MATCH (p:Permission) RETURN p { Name: p.Name } ORDER BY p.Name";
            var users = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "p");

            return users;
        }
    }
}