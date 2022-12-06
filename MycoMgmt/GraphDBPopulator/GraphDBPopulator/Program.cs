using System;
using System.Linq;
using Neo4j.Driver;

namespace GraphDBPopulator
{
    public class HelloWorldExample : IDisposable
    {
        private bool _disposed;
        private readonly IDriver _driver;

        ~HelloWorldExample() => Dispose(false);

        public HelloWorldExample(string uri, string user, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

        public void PrintGreeting(string message)
        {
            using var session = _driver.Session();
            var greeting = session.ExecuteWrite(tx =>
            {
                var result = tx.Run("CREATE (a:Greeting) " +
                                    "SET a.message = $message " +
                                    "RETURN a.message + ', from node ' + id(a)",
                    new {message});
                return result.Single()[0].As<string>();
            });
            Console.WriteLine(greeting);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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

        public static void Main()
        {
            using var greeter = new HelloWorldExample("bolt://localhost:7687", "neo4j", "sql");

            greeter.PrintGreeting("hello, world");
        }
    }
}