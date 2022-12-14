using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MycoMgmt.API.DataStores.Neo4J;
using MycoMgmt.Domain.Models.UserManagement;
using Neo4j.Driver;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace MycoMgmt.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly INeo4JDataAccess _neo4JDataAccess;
        private ILogger<UserRepository> _logger;
        
        public UserRepository(INeo4JDataAccess neo4JDataAccess, ILogger<UserRepository> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }
        
        public async Task<string> Add(User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Name))
                throw new ArgumentNullException(nameof(user), "User must not be null");

            return await PersistToDatabase(user);
        }

        private async Task<string> PersistToDatabase(User user)
        {
            try
            {
                var queryList = CreateQueryList(user);
                var result = await _neo4JDataAccess.RunTransaction(queryList);
                return result;
            }
            catch (ClientException ex)
            {
                if (!Regex.IsMatch(ex.Message, @"Node\(\d+\) already exists with *"))
                    throw;

                return JsonConvert.SerializeObject(new { Message = $"A User already exists with the name {user.Name}" });
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        private static List<string> CreateQueryList(User user)
        {
            var queryList = new List<string>
            {
                $@"
                        MERGE (u:User {{ Name: '{user.Name}' }}) 
                        RETURN u;
                    ",
                $@"
                        MATCH 
                            (u:User    {{ Name: '{user.Name}'    }}),
                            (a:Account {{ Name: '{user.Account}' }})
                        MERGE
                            (a)-[r:HAS]->(u)
                        RETURN r
                    ",
                $@"
                        MATCH 
                            (u:User {{ Name: '{user.Name}'    }}),
                            (d:Day  {{ day: {user.CreatedOn.Day} }})<-[:HAS_DAY]-(:Month {{ month: {user.CreatedOn.Month} }})<-[:HAS_MONTH]-(:Year {{ year: {user.CreatedOn.Year} }})
                        MERGE
                            (u)-[r:CREATED_ON]->(d)
                        RETURN r
                    ",
                $@"
                        MATCH 
                            (nu:User {{ Name: '{user.Name}'      }}),
                            (cb:User {{ Name: '{user.CreatedBy}' }})
                        MERGE
                            (cb)-[r:CREATED]->(nu)
                        RETURN r
                    "
            };

            if (user.Permissions != null)
            {
                foreach (var permission in user.Permissions)
                {
                    queryList.Add($@"
                            MATCH
                                (u:User       {{ Name: '{user.Name}'  }}),
                                (p:Permission {{ Name: '{permission}' }})
                            MERGE
                                (u)-[r:HAS]->(p)
                            RETURN r
                        ");
                }
            }

            if (user.Roles != null)
            {
                foreach (var role in user.Roles)
                {
                    queryList.Add($@"
                            MATCH
                                (u:User    {{ Name: '{user.Name}' }}),
                                (r:IAMRole {{ Name: '{role}' }})
                            MERGE
                                (u)-[rel:HAS]->(r)
                            RETURN r
                        ");
                }
            }

            return queryList;
        }

        public async Task<List<Dictionary<string, object>>> GetAll()
        {
            const string query = @"MATCH (u:User) RETURN u { Name: u.Name } ORDER BY u.Name";
            var users = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "u");

            return users;
        }
    }
}