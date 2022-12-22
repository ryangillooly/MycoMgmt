#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MycoMgmt.API.DataStores.Neo4J;
using Neo4j.Driver;
using Newtonsoft.Json;

namespace MycoMgmt.DataStores.Neo4J;

public class Neo4JDataAccess : INeo4JDataAccess
{
    private readonly IAsyncSession _session;
    private readonly IDriver _driver;
    private readonly ILogger<Neo4JDataAccess> _logger;
    private readonly string _database;

    public Neo4JDataAccess(IDriver driver, ILogger<Neo4JDataAccess> logger, IOptions<Neo4JSettings> appSettingsOptions)
    {
        _logger = logger;
        _database = appSettingsOptions.Value.Neo4JDatabase ?? "neo4j";
        _driver = driver;
        _session = driver.AsyncSession(o => o.WithDatabase(_database));
    }
        
    public async Task<List<object>> ExecuteReadListAsync(string query, string returnObjectKey, IDictionary<string, object>? parameters = null)
    {
        return await ExecuteReadTransactionAsync(query, returnObjectKey, parameters);
    }
        
    public async Task<List<object>> ExecuteReadDictionaryAsync(string query, string returnObjectKey, IDictionary<string, object>? parameters = null)
    {
        return await ExecuteReadTransactionAsync(query, returnObjectKey, parameters);
    }
        
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
        
    public async Task<string> ExecuteWriteTransactionAsync<T>(string query, IDictionary<string, object>? parameters = null)
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
                    var res   = await tx.RunAsync(query);
                    var scalarObj = await res.SingleAsync();
                    var scalar     = scalarObj[0];

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
        
    private async Task<List<object>> ExecuteReadTransactionAsync(string query, string returnObjectKey, IDictionary<string, object>? parameters)
    {
        try
        {                
            var result = await _session.ExecuteReadAsync(async tx =>
            {
                var res = await tx.RunAsync(query, parameters);
                var records = await res.ToListAsync();
                var data = records.Select(x => x.Values[returnObjectKey]);

                return data.ToList();
            });

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was a problem while executing database query");
            throw;
        }
    }
        
    /*
    public async Task<List<object>> GetNodesAsync(string query, string objKey)
    {
        // Create a new session using the driver
        await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
        
        // Execute a Cypher query that returns a list of nodes
        var result = session.RunAsync(query);
        
        // Iterate through the results and add the nodes to a list
        var taskResult = result.Result;

        var nodes = await taskResult.ToListAsync(record => record[objKey]);

        return nodes;
    }
    */
        
    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await _session.CloseAsync();
    }
}