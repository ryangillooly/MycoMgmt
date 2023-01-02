﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.Infrastructure.DataStores.Neo4J;
using Neo4j.Driver;
using Newtonsoft.Json;
#pragma warning disable CS8604

// ReSharper disable once CheckNamespace
namespace MycoMgmt.Infrastructure.Repositories
{
    public class AccountRepository : BaseRepository<Account>
    {
        private readonly INeo4JDataAccess _neo4JDataAccess;
        private ILogger<AccountRepository> _logger;
        
        public AccountRepository(INeo4JDataAccess neo4JDataAccess, ILogger<AccountRepository> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }
        
        public override async Task<string> SearchByName(Account account)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(account.SearchByNameQuery(), "x");
            return JsonConvert.SerializeObject(result);
        }

        public override async Task<string> GetByName(Account account)
        {
            var result = await _neo4JDataAccess.ExecuteReadDictionaryAsync(account.GetByNameQuery(), "x");

            return JsonConvert.SerializeObject(result);
        }

        public override async Task<string> GetById(Account account)
        {
            var result = await _neo4JDataAccess.ExecuteReadScalarAsync<INode>(account.GetByIdQuery());
            return JsonConvert.SerializeObject(result);
        }

        public override async Task<string> GetAll(Account account, int skip, int limit)
        {
            var result = await _neo4JDataAccess.ExecuteReadListAsync(account.GetAllQuery(skip, limit), "result");
            return JsonConvert.SerializeObject(result);
        }

        public override async Task<List<IEntity>> Create(Account account)
        {
            var queryList = account.CreateQueryList();
            var result = await _neo4JDataAccess.RunTransaction(queryList);
            return result;
        }
        
        public override async Task<string> Delete(Account account)
        {
            var accounts = await _neo4JDataAccess.RunTransaction(new List<string?> { account.Delete() });
            return JsonConvert.SerializeObject(accounts);
        }

        public override async Task<string> Update(Account account)
        {
            var queryList = account.UpdateQueryList();
            var cultures = await _neo4JDataAccess.RunTransaction(queryList);
            return JsonConvert.SerializeObject(cultures);
        }
    }
}