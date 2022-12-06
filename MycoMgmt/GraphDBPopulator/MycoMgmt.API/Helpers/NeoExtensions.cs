using System.Collections.Generic;
using MycoMgmt.API.Models;
using Neo4j.Driver;

namespace MycoMgmt.API.Helpers
{
    public static class NeoExtensions
    {
        public static void CreateCulture(this IAsyncSession session, string name, string isSuccessful, string type, string isPurchase) =>
            session.WriteToDatabase($@"MERGE (:Culture{isSuccessful}{type}{isPurchase} {{ Name: '{name}' }});");
        
        public static void CreateLocation(this IAsyncSession session, string name) =>
            session.WriteToDatabase($@"MERGE (l:Location {{ Name: '{name}'}})");
        
        public static IList<IRecord> WriteToDatabase(this IAsyncSession session, string query) => session.ExecuteWriteAsync(async tx => 
        {
            var r = await tx.RunAsync(query);
            return await r.ToListAsync();    
        }).Result;
        
        public static IList<IRecord> ReadFromDatabase(IAsyncSession session, string query) => session.ReadTransactionAsync(async tx => 
        {
            var r = await tx.RunAsync(query);
            return await r.ToListAsync();
        }).Result;
    }
}