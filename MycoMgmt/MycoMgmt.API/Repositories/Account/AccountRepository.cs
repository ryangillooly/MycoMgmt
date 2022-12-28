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
        
        public async Task<string> SearchByName(Account account)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(account.SearchByNameQuery(), "x");
            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> GetByName(Account account)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(account.GetByNameQuery(), "x");

            return JsonConvert.SerializeObject(result);
        }

        public async Task<string> GetById(Account account)
        {
            var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(account.GetByIdQuery());
            return JsonConvert.SerializeObject(result);
        }
    
        public async Task<string> GetAll(Account account, int? skip, int? limit)
        {
            skip  = skip ?? 0;
            limit = limit ?? 0;
            
            var result = await _neo4JDataAccess.ExecuteReadListAsync(account.GetAll(skip, limit), "x");
            return JsonConvert.SerializeObject(result);
        }
        
        public async Task<string> Create(Account account)
        {
            var queryList = new List<string?>
            {
                account.Create(),
                account.CreateCreatedRelationship(),
                account.CreateCreatedOnRelationship()
            };
            
            var result = await _neo4JDataAccess.RunTransaction(queryList);
            return result;
        }
        
        public async Task<string> Delete(Account account)
        {
            var accounts = await _neo4JDataAccess.RunTransaction(new List<string?> { account.Delete() });
            return JsonConvert.SerializeObject(accounts);
        }

        public async Task<string> Update(Account account)
        {
            var queryList = new List<string?>
            {
                account.UpdateName(),
                account.UpdateModifiedOnRelationship(),
                account.UpdateModifiedRelationship()
            };

            var cultures = await _neo4JDataAccess.RunTransaction(queryList);
            return JsonConvert.SerializeObject(cultures);
        }
    }
}