#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo4j.Driver;
using Newtonsoft.Json;

namespace MycoMgmt.API.DataStores.Neo4J
{
    public class Neo4JDataAccess : INeo4JDataAccess
    {
        private readonly IAsyncSession _session;
        private readonly ILogger<Neo4JDataAccess> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Neo4JDataAccess"/> class.
        /// </summary>
        public Neo4JDataAccess(IDriver driver, ILogger<Neo4JDataAccess> logger, IOptions<Neo4JSettings> appSettingsOptions)
        {
            _logger = logger;
            var database = appSettingsOptions.Value.Neo4jDatabase ?? "neo4j";
            _session = driver.AsyncSession(o => o.WithDatabase(database));
        }

        /// <summary>
        /// Execute read list as an asynchronous operation.
        /// </summary>
        public async Task<List<string>> ExecuteReadListAsync(string query, string returnObjectKey, IDictionary<string, object>? parameters = null)
        {
            return await ExecuteReadTransactionAsync<string>(query, returnObjectKey, parameters);
        }

        /// <summary>
        /// Execute read dictionary as an asynchronous operation.
        /// </summary>
        public async Task<List<Dictionary<string, object>>> ExecuteReadDictionaryAsync(string query, string returnObjectKey, IDictionary<string, object>? parameters = null)
        {
            return await ExecuteReadTransactionAsync<Dictionary<string, object>>(query, returnObjectKey, parameters);
        }

        /// <summary>
        /// Execute read scalar as an asynchronous operation.
        /// </summary>
        public async Task<T> ExecuteReadScalarAsync<T>(string query, IDictionary<string, object>? parameters = null)
        {
            try
            {
                parameters = parameters ?? new Dictionary<string, object>();

                var result = await _session.ExecuteReadAsync(async tx =>
                {
                    var res = await tx.RunAsync(query, parameters);

                    var scalar = (await res.SingleAsync())[0].As<T>();

                    return scalar;
                });

                return result;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex);
               throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was a problem while executing database query");
                throw;
            }
        }

        /// <summary>
        /// Execute write transaction
        /// </summary>
        public async Task<string> ExecuteWriteTransactionAsync<T>(string query,
            IDictionary<string, object>? parameters = null)
        {
            try
            {
                parameters = parameters ?? new Dictionary<string, object>();

                var result = await _session.ExecuteWriteAsync(async tx =>
                {
                    var res = await tx.RunAsync(query, parameters);

                    var scalar = (await res.SingleAsync())[0].As<T>();

                    return scalar;
                });

                return JsonConvert.SerializeObject(result);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message != "The result is empty.") 
                    throw;
            
                return JsonConvert.SerializeObject(new { Message = $"No results were found for Culture" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was a problem while executing database query");
                throw;
            }
        }
        
        public async Task<string> RunTransaction(List<string> queryList)
        {
            try
            {
                var result = await _session.ExecuteWriteAsync(async tx =>
                {
                    var returnList = new List<string>();
                    
                    foreach(var query in queryList)
                    {
                        var res = await tx.RunAsync(query);

                        var scalar = (await res.SingleAsync())[0];

                        returnList.Add(JsonConvert.SerializeObject(scalar));
                    }
                    return returnList;
                });

                return JsonConvert.SerializeObject(result);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message != "The result is empty.") 
                    throw;
            
                return JsonConvert.SerializeObject(new { Message = $"No results were provided by the database" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was a problem while executing database query");
                throw;
            }
        }

        /// <summary>
        /// Execute read transaction as an asynchronous operation.
        /// </summary>
        private async Task<List<T>> ExecuteReadTransactionAsync<T>(string query, string returnObjectKey, IDictionary<string, object>? parameters)
        {
            try
            {                
                parameters = parameters ?? new Dictionary<string, object>();

                var result = await _session.ExecuteReadAsync(async tx =>
                {
                    var res = await tx.RunAsync(query, parameters);
                    var records = await res.ToListAsync();
                    var data = records.Select(x => (T) x.Values[returnObjectKey]).ToList();

                    return data;
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was a problem while executing database query");
                throw;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources asynchronously.
        /// </summary>
        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            await _session.CloseAsync();
        }
    }
}