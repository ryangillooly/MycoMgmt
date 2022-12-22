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
    public class AccountRepository : IAccountRepository
    {
        private readonly INeo4JDataAccess _neo4JDataAccess;
        private ILogger<AccountRepository> _logger;
        
        public AccountRepository(INeo4JDataAccess neo4JDataAccess, ILogger<AccountRepository> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }
        
        public async Task<string> CreateAsync(Account account)
        {
            if (account == null || string.IsNullOrWhiteSpace(account.Name))
                throw new ArgumentNullException(nameof(account), "Account must not be null");

            return await PersistToDatabase(account);
        }
        public async Task<string> DeleteAsync(long id)
        {
            var query = new List<string>{ $"MATCH (a:Account) WHERE ID(a) = { id } DETACH DELETE a RETURN a" };
            var accounts = await _neo4JDataAccess.RunTransaction(query);

            return accounts is null ? null : JsonConvert.SerializeObject(accounts);
        }

        public async Task<string> UpdateAsync(Account account)
        {
            // Fix this query. Placeholder put in for the time being until I get round to it
            var query = new List<string> { $"MATCH (a:Account {{ Name: '{account.Name}' }}) DETACH DELETE a RETURN a" };
            var accounts = await _neo4JDataAccess.RunTransaction(query);

            return accounts is null ? null : JsonConvert.SerializeObject(accounts);
        }
        private async Task<string> PersistToDatabase(Account account)
        {
            try
            {
                var queryList = CreateQueryList(account);
                var result = await _neo4JDataAccess.RunTransaction(queryList);
                return result;
            }
            catch (ClientException ex)
            {
                if (!Regex.IsMatch(ex.Message, @"Node\(\d+\) already exists with *"))
                    throw;

                return JsonConvert.SerializeObject(new { Message = $"An Account already exists with the name {account.Name}" });
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        private static List<string> CreateQueryList(Account account)
        {
            var queryList = new List<string>
            {
                $@"
                        MERGE 
                        (
                            a:Account
                            {{
                                Name:  '{account.Name}'
                            }}        
                        ) 
                        RETURN a;
                    "
            };
            return queryList;
        }
        public async Task<string> GetAllAsync()
        {
            const string query = "MATCH (a:Account) RETURN a ORDER BY a.Name";
            var accounts = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "a");

            return accounts is null ? null : JsonConvert.SerializeObject(accounts);
        }
    }
}