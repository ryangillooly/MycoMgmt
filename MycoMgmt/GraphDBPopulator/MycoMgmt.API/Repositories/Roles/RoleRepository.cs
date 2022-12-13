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
    public class RoleRepository : IRoleRepository
    {
        private readonly INeo4JDataAccess _neo4JDataAccess;
        private ILogger<RoleRepository> _logger;
        
        public RoleRepository(INeo4JDataAccess neo4JDataAccess, ILogger<RoleRepository> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }
        
        public async Task<string> AddRole(IAMRole role)
        {
            if (role == null || string.IsNullOrWhiteSpace(role.Name))
                throw new ArgumentNullException(nameof(role), "Role must not be null");

            try
            {
                var queryList = new List<string>
                {
                    $@"
                        MERGE 
                        (
                            r:IAMRole
                            {{
                                Name: '{role.Name}'
                            }}        
                        ) 
                        RETURN r;
                    "
                };

                if (role.Permissions != null)
                {
                    foreach (var permission in role.Permissions)
                    {
                        queryList.Add($@"
                            MATCH 
                                (r:IAMRole {{ Name: '{ role.Name }' }}),
                                (p:Permission {{ Name: '{ permission }' }})
                            MERGE
                                (r)-[rel:HAS]->(p)
                            RETURN rel
                        ");
                    }
                }

                var result = await _neo4JDataAccess.RunTransaction(queryList);

                return result;
            }
            catch (ClientException ex)
            {
                if (!Regex.IsMatch(ex.Message, @"Node\(\d+\) already exists with *"))
                    throw;
                
                return JsonConvert.SerializeObject(new { Message = $"A Role already exists with the name { role.Name }" });
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        
        public async Task<List<Dictionary<string, object>>> GetAllRoles()
        {
            const string query = @"MATCH (r:IAMRole) RETURN r { Name: r.Name } ORDER BY r.Name";
            var users = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "r");

            return users;
        }
    }
}