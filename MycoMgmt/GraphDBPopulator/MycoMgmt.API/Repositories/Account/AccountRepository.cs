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
    public class AccountRepository : IAccountRepository
    {
        private readonly INeo4JDataAccess _neo4JDataAccess;
        private ILogger<AccountRepository> _logger;
        
        public AccountRepository(INeo4JDataAccess neo4JDataAccess, ILogger<AccountRepository> logger)
        {
            _neo4JDataAccess = neo4JDataAccess;
            _logger = logger;
        }
        
        public async Task<string> AddAccount(Account account)
        {
            if (account == null || string.IsNullOrWhiteSpace(account.Name))
                throw new ArgumentNullException(nameof(account), "Account must not be null");

            try
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

                var result = await _neo4JDataAccess.RunTransaction(queryList);

                return result;
            }
            catch (ClientException ex)
            {
                if (!Regex.IsMatch(ex.Message, @"Node\(\d+\) already exists with *"))
                    throw;
                
                return JsonConvert.SerializeObject(new { Message = $"An Account already exists with the name { account.Name }" });
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        
        public async Task<List<Dictionary<string, object>>> GetAllAccounts()
        {
            const string query = @"MATCH (a:Account) RETURN a { Name: a.Name } ORDER BY a.Name";
            var accounts = await _neo4JDataAccess.ExecuteReadDictionaryAsync(query, "a");

            return accounts;
        }
    }
}