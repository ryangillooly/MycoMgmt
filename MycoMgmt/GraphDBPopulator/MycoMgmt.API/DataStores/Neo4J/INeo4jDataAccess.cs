﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MycoMgmt.API.DataStores.Neo4J
{
    public interface INeo4JDataAccess : IAsyncDisposable
    {
        Task<List<string>> ExecuteReadListAsync(string query, string returnObjectKey, IDictionary<string, object>? parameters = null);
       
        Task<List<Dictionary<string, object>>> ExecuteReadDictionaryAsync(string query, string returnObjectKey, IDictionary<string, object>? parameters = null);

        Task<T> ExecuteReadScalarAsync<T>(string query, IDictionary<string, object>? parameters = null);

        Task<string> ExecuteWriteTransactionAsync<T>(string query, IDictionary<string, object> parameters = null);

        Task<string> RunTransaction(List<string> queryList);
    }
}