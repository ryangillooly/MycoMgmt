using System;
using System.Collections.Generic;
using MycoMgmt.API.Models;
using Neo4j.Driver;

namespace MycoMgmt.API.Helpers
{
    public static class NeoExtensions
    {
        public static void CreateCulture(this IAsyncSession session, string id, string name, string isSuccessful, string type, string isPurchase) =>
            session.WriteToDatabase($@"MERGE (:Culture{isSuccessful}{type}{isPurchase} {{ UUID: '{id}', Name: '{name}' }});");
        // Need to deal with the UUID aspect, as this is forcing us to create new Cultures in the DB, as when you Merge, the existing culture is different, via the UUID
        
        public static void CreateLocation(this IAsyncSession session, string id, string name) =>
            session.WriteToDatabase($@"MERGE (:Location {{ UUID: '{id}', Name: '{name}'}})");
        
        public static void CreateRecipe(this IAsyncSession session, string id, string name, string type, string desc, string steps) =>
            session.WriteToDatabase($@"MERGE (:Recipe {{  UUID: '{id}', Name: '{name}', Type: '{type}', Description: '{desc}', Steps: '{steps}' }})");
        
        public static void CreateStrain(this IAsyncSession session, string id, string name) =>
            session.WriteToDatabase($@"MERGE (:Strain {{ UUID: '{id}', Name: '{name}', Effects: ['Visuals','Calming','Oneness']}})");
        
        public static void CreateParentRelationship(this IAsyncSession session, string child, string parent) =>
            session.WriteToDatabase($@"MATCH (c:{child}), (p:{parent}) MERGE (c)-[:HAS_PARENT]->(p)");

        public static void GetCultures(this IAsyncSession session) =>
            session.ReadFromDatabase($@"MATCH (c:Culture) RETURN c");

        public static IList<IRecord> WriteToDatabase(this IAsyncSession session, string query) => session.ExecuteWriteAsync(async tx => 
        {
            var r = await tx.RunAsync(query);
            return await r.ToListAsync();    
        }).Result;
        
        public static IList<IRecord> ReadFromDatabase(this IAsyncSession session, string query) => session.ReadTransactionAsync(async tx => 
        {
            var r = await tx.RunAsync(query);
            return await r.ToListAsync();
        }).Result;
    }
}