using System;
using System.Linq;
using Neo4j.Driver;

namespace MycoMgmt.Populator
{
    public class Neo4J : IDisposable
    {
        private bool _disposed;
        private readonly IDriver _driver;

        ~Neo4J() => Dispose(false);

        public Neo4J(string uri, string user, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        public async void PrintGreeting(string message)
        {
            await using var session = _driver.AsyncSession();

            try
            {
                var greeting =   session.ExecuteWriteAsync(tx =>
                                            {
                                                var result = tx.RunAsync("CREATE (a:Greeting {message: '$message'}) RETURN a.message + ', from node ' + id(a)", new { message });
                                                return result.Result.ToListAsync(r => r.As<string>());
                                            });

                
                Console.WriteLine(string.Join(",", greeting.Result.ToArray()));
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _driver?.Dispose();
            }

            _disposed = true;
        }
    }
}