using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MycoMgmt.API.DataStores.Neo4J;
using MycoMgmt.API.Helpers;
using MycoMgmt.Domain.Models.UserManagement;
using Neo4j.Driver;
using Newtonsoft.Json;
#pragma warning disable CS8604

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
        
        public async Task<string> Create(Account account)
        {
            if (account == null || string.IsNullOrWhiteSpace(account.Name))
                throw new ArgumentNullException(nameof(account), "Account must not be null");

            var queryList = new List<string>
            {
                account.Create(),
                account.CreateCreatedRelationship(),
                account.CreateCreatedOnRelationship()
            };
            
            var result = await _neo4JDataAccess.RunTransaction(queryList);
            return result;
        }
        
        public async Task<string> Delete(string elementId)
        {
            var query = new List<string>{ $"MATCH (a:Account) WHERE elementId(a) = '{elementId}' DETACH DELETE a RETURN a" };
            var accounts = await _neo4JDataAccess.RunTransaction(query);

            return JsonConvert.SerializeObject(accounts);
        }

        public async Task<string> Update(Account account, string elementId)
        {
            var queryList = new List<string>
            {
                $"MATCH (a:Account) WHERE elementId(a) = '{elementId}' SET a.Name = '{account.Name}' RETURN a",
                account.UpdateModifiedOnRelationship(elementId),
                account.UpdateModifiedRelationship(elementId)
            };

            var cultures = await _neo4JDataAccess.RunTransaction(queryList);
            return JsonConvert.SerializeObject(cultures, Formatting.Indented);
        }

        public async Task<string> GetAll()
        {
            const string query = "MATCH (a:Account) RETURN a ORDER BY a.Name";
            var accounts = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "a");

            return JsonConvert.SerializeObject(accounts);
        }
    }
}