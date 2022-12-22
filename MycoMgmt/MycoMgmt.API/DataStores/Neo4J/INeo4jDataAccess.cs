// ReSharper disable once CheckNamespace
namespace MycoMgmt.API.DataStores.Neo4J;

public interface INeo4JDataAccess : IAsyncDisposable
{
    Task<List<object>> ExecuteReadListAsync(string query, string returnObjectKey, IDictionary<string, object>? parameters = null);

    Task<List<object>> ExecuteReadDictionaryAsync(string query, string returnObjectKey,
        IDictionary<string, object>? parameters = null);

    Task<T> ExecuteReadScalarAsync<T>(string query, IDictionary<string, object>? parameters = null);

    Task<T> ExecuteWriteTransactionAsync<T>(string query, IDictionary<string, object>? parameters = null);

    Task<string> RunTransaction(List<string> queryList);

    //Task<List<object>> GetNodesAsync(string query, string objKey);
}