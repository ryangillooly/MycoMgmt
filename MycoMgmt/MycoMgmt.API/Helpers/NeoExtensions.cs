using System.Collections.Generic;
using MycoMgmt.Domain.Models;
using MycoMgmt.Domain.Models.Mushrooms;
using Neo4j.Driver;

namespace MycoMgmt.API.Helpers
{
    public static class NeoExtensions
    {
        // public static void CreateCulture(this IAsyncSession session, string id, string name, string isSuccessful, string type, string isPurchase) =>
         //   session.WriteToDatabase($@"MERGE (:Culture{isSuccessful}{type}{isPurchase} {{ UUID: '{id}', Name: '{name}' }});");
        // Need to deal with the UUID aspect, as this is forcing us to create new Cultures in the DB, as when you Merge, the existing culture is different, via the UUID

        public static void CreateCulture(this IAsyncSession session, Culture culture)
        {
            var isSuccessful = culture.IsSuccessful();
            var isPurchase = culture.IsPurchase();
            var type = ":" + culture.Type;
            var name = culture.Name;
            var location = culture.Location?.ToString();
            var strain = culture.Strain.ToString();
        
            var createQuery = $@"
                MERGE (c:Culture{isSuccessful}{type}{isPurchase} {{ Name: '{name}' }})
                MERGE (l:Location {{ Name: '{location}'}})
                MERGE (s:Strain {{ Name: '{strain}' }})
                MERGE (c)-[:STORED_IN]-(l)
                MERGE (c)-[:IS_STRAIN]-(s)
            ";
        
            session.WriteToDatabase(createQuery);
        
            if (name != null)
                session.WriteToDatabase($"CREATE CONSTRAINT culture_{name.Replace("-","_")}      IF NOT EXISTS FOR (c:Culture)  REQUIRE c.Name IS UNIQUE");
        
            if (location != null)
                session.WriteToDatabase($"CREATE CONSTRAINT location_{location} IF NOT EXISTS FOR (l:Location) REQUIRE l.Name IS UNIQUE");

            session.WriteToDatabase($"CREATE CONSTRAINT strain_{strain}     IF NOT EXISTS FOR (s:Strain)   REQUIRE s.Name IS UNIQUE");
            
        }
        
        
        public static void CreateLocation(this IAsyncSession session, string id, string name) =>
            session.WriteToDatabase($@"MERGE (:Location {{ UUID: '{id}', Name: '{name}'}})");
        
        public static void CreateRecipe(this IAsyncSession session, string id, string name, string type, string desc, string steps) =>
            session.WriteToDatabase($@"MERGE (:Recipe {{  UUID: '{id}', Name: '{name}', Type: '{type}', Description: '{desc}', Steps: '{steps}' }})");
        
        public static void CreateStrain(this IAsyncSession session, string id, string name) =>
            session.WriteToDatabase($@"MERGE (:Strain {{ UUID: '{id}', Name: '{name}', Effects: ['Visuals','Calming','Oneness']}})");
        
        public static void CreateParentRelationship(this IAsyncSession session, string child, string parent) =>
            session.WriteToDatabase($@"MATCH {child}, {parent} MERGE (c)-[:HAS_PARENT]->(n)");
        public static void GetCultures(this IAsyncSession session) =>
            session.ReadFromDatabase($@"MATCH (c:Culture) RETURN c");

        public static IList<IRecord> WriteToDatabase(this IAsyncSession session, string query) => session.ExecuteWriteAsync(async tx => 
        {
            var r = await tx.RunAsync(query);
            return await r.ToListAsync();    
        }).Result;
        
        public static IList<IRecord> ReadFromDatabase(this IAsyncSession session, string query) => session.ExecuteReadAsync(async tx => 
        {
            var r = await tx.RunAsync(query);
            return await r.ToListAsync();
        }).Result;
        
        public static string IsSuccessful(this Culture culture)
        {
            if ((bool)!culture.Finished) return string.Empty;

            return culture.Successful switch
            {
                true => ":Successful",
                false => ":Failed"
            };
        }

        public static string? IsPurchase(this Culture culture) => culture.Vendor is null ? null : ":Purchase";
        
        public static void CreateLocation(this IAsyncSession session, Location location)
        {
            session.WriteToDatabase($@"MERGE (:Location {{ Name: '{ location.Name }', AgentConfigured: '{ location.AgentConfigured }' }})");
        }
    }
}