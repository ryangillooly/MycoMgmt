using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading.Tasks;
using MycoMgmt.API.Helpers;
using MycoMgmt.API.Models;
using MycoMgmt.Populator.Models;
using Neo4j.Driver;
using Neo4j.Driver.Extensions;
using Neo4jClient.Extension.Cypher;
using Neo4jClient.Extensions;
using Newtonsoft.Json;
using static MycoMgmt.Populator.Extensions;

namespace MycoMgmt.Populator
{
    public class Neo4JPopulator
    {
        private bool _disposed;
        private readonly IDriver _driver;
        
        public static async Task Main()
        {
            var driver = GraphDatabase.Driver("neo4j://localhost:7687", AuthTokens.Basic("test", "test"));
            var session = driver.AsyncSession(o => o.WithDatabase("test"));
            
            /*
            session.CreateNeoTimeTree();
            session.CreateRolesAndPermissions();
            session.CreateNodes();
            session.CreateRelationships();
            session.TestReads();
            */

           /* var culture = new Culture
            {
                Name = "Sp-01",
                Type = CultureTypes.Agar,
                Strain = Strain.GoldenTeacher,
                Location = Locations.GrowTent,
                Vendor = new Vendor{ Name = "MycoPunks", URL = new Uri("http://mycopunks.com")},
                Finished = false
            };

            var location = new Location
            {
                Name = "GrowTent",
                Created = new Created { On = DateTime.Now, By = Users.Ryan}
            };
                
            session.CreateLocation(location);
           // session.CreateCulture(culture);
            */
            await session?.CloseAsync();
        }
    }

    public class Created
    {
        public DateTime On { get; set; }
        public Users By { get; set; }
    }
    
    public class Modified
    {
        public DateTime On { get; set; }
        public User By { get; set; }
    }
    
    public static class Extensions
    {
        

        public static string IsSuccessful(this Culture culture)
        {
            if (!culture.Finished) return string.Empty;

            return culture.Successful switch
            {
                true => ":Successful",
                false => ":Failed"
            };
        }

        public static string IsPurchase(this Culture culture) => culture.Vendor is null ? null : ":Purchase";
        
        public static void CreateLocation(this IAsyncSession session, Location location)
        {
            session.WriteToDatabase($@"MERGE (:Location {{ Name: '{ location.Name }', AgentConfigured: '{ location.AgentConfigured }' }})");
        }
       /* public static void CreateCulture(this IAsyncSession session, Culture culture)
        {
        

            session.WriteToDatabase($@"MERGE (:Culture{culture.IsSuccessful()}{":" + culture.Type}{culture.IsPurchase()} {{ Name: '{culture.Name}' }});");
            session.WriteToDatabase($@"
                MATCH 
                    (c:Culture{culture.IsSuccessful()}{":" + culture.Type}{culture.IsPurchase()} {{ Name: '{ culture.Name }' }}),
                    (l:Location {{ Name: '{ culture.Location }'}})
                MERGE
                    (c)-[:STORED_IN]->(l)
            ");

            session.WriteToDatabase($@"
                MATCH 
                    (c:Culture{culture.IsSuccessful()}{":" + culture.Type}{culture.IsPurchase()} {{ Name: '{ culture.Name }' }}),
                    (p:)
                MERGE
                    (c)-[:HAS_PARENt]->(p)
            ");
        }*/
        
        public static void CreateNeoTimeTree(this IAsyncSession session)
        {
            session.WriteToDatabase(
          $@"// Create DateTime Tree / Index
                WITH range(2021, 2024) AS years, range(1,12) as months
                FOREACH(year IN years |
                    CREATE (y:Year {{year: year}})
                    FOREACH(month IN months |
                        CREATE (m:Month {{month: month}})
                        CREATE (y)-[:HAS_MONTH]->(m)
                        FOREACH(day IN (CASE
                            WHEN month IN [1,3,5,7,8,10,12] THEN range(1,31)
                            WHEN month = 2 
                                THEN CASE
                                    WHEN year % 4 <> 0 THEN range(1,28)
                                    WHEN year % 100 <> 0 THEN range(1,29)
                                    WHEN year % 400 = 0 THEN range(1,29)
                                    ELSE range(1,28)
                                END
                            ELSE 
                                range(1,30)
                        END) |
                        CREATE (d:Day {{day: day}})
                        CREATE (m)-[:HAS_DAY]->(d))))

                WITH *

                MATCH (year:Year)-[:HAS_MONTH]->(month)-[:HAS_DAY]->(day)
                WITH year,month,day
                ORDER BY year.year, month.month, day.day
                WITH collect(day) as days
                FOREACH(i in RANGE(0, size(days)-2) |
                    FOREACH(day1 in [days[i]] |
                        FOREACH(day2 in [days[i+1]] |
                            CREATE (day1)-[:NEXT]->(day2))))");

        }

        public static void CreateNodes(this IAsyncSession session)
        {
            session.WriteToDatabase($"MERGE (:Account {{  Name: 'RG Ltd.'}})");
            session.WriteToDatabase($"MERGE (:Vendor {{  Name: 'MycoPunks', Url: 'https://mycopunks.com'}})");
            session.WriteToDatabase($"MERGE (:User {{  Name: 'Ryan' }})");
            session.WriteToDatabase($"MERGE (:User {{  Name: 'Calum' }})");
            session.WriteToDatabase($"MERGE (:User {{  Name: 'Aiden' }})");
            session.WriteToDatabase($"MERGE (:Location {{  Name: 'Grow Tent', AgentConfigured: 'false' }})");
            session.WriteToDatabase($"MERGE (:Location {{  Name: 'Incubation Chamber', AgentConfigured: 'true' }})");
            session.WriteToDatabase($"MERGE (:Location {{  Name: 'Fruiting Chamber', AgentConfigured: 'false' }})");
            session.WriteToDatabase($"MERGE (:Location {{  Name: 'Fruiting Chamber', AgentConfigured: 'false' }})");
            session.WriteToDatabase($"MERGE (:Recipe {{  Name: 'Popcorn Spawn', Type: 'Grain Spawn', Description: 'Madddddd', Steps: '1.... 2.... 3....' }})");
            session.WriteToDatabase($"MERGE (:Recipe {{  Name: 'Coco Coir Bulk', Type: 'Bulk', Description: 'Madddddd', Steps: '1.... 2.... 3....' }})");
            session.WriteToDatabase($"MERGE (:Recipe {{  Name: 'CVG Bulk', Type: 'Bulk', Description: 'Madddddd', Steps: '1.... 2.... 3....' }})");
            session.WriteToDatabase($"MERGE (:Recipe {{  Name: 'Blue Transparent Agar', Type: 'Agar', Description: 'Madddddd', Steps: '1.... 2.... 3....' }})");
            session.WriteToDatabase($"MERGE (:Ingredient {{ Name: 'Popcorn' }})");
            session.WriteToDatabase($"MERGE (:Ingredient {{ Name: 'Gypsum' }})");
            session.WriteToDatabase($"MERGE (:Ingredient {{ Name: 'Agar' }})");
            session.WriteToDatabase($"MERGE (:Ingredient {{ Name: 'Food Colouring' }})");
            session.WriteToDatabase($"MERGE (:Ingredient {{ Name: 'Vermiculite' }})");
            session.WriteToDatabase($"MERGE (:Ingredient {{ Name: 'Coco-Coir' }})");
            session.WriteToDatabase($"MERGE (:Strain {{ Name: 'Golden Teacher', Effects: ['Visuals','Calming','Oneness']}})");
            session.WriteToDatabase($"MERGE (:Strain {{ Name: 'B+', Effects: ['Visuals','Calming','Oneness']}})");
            session.WriteToDatabase($"MERGE (:Strain {{ Name: 'Mazapatec', Effects: ['Visuals','Calming','Oneness']}})");
            session.WriteToDatabase($"MERGE (:Culture:SporePrint:Purchase {{ Name: 'SP-GT-01'}})");
            session.WriteToDatabase($"MERGE (:Culture:SporePrint {{ Name: 'SP-BP-01'}})");
            session.WriteToDatabase($"MERGE (:Culture:Agar {{ Name: 'SP-GT-01-01'}})");
            session.WriteToDatabase($"MERGE (:Culture:Agar {{ Name: 'SP-GT-01-02'}})");
            session.WriteToDatabase($"MERGE (:Culture:Agar {{ Name: 'SP-GT-01-03'}})");
            session.WriteToDatabase($"MERGE (:Culture:Agar {{ Name: 'SP-BP-01-01'}})");
            session.WriteToDatabase($"MERGE (:Spawn:Failed {{ Name: 'SPN-GT-01-01', Type: 'Popcorn', Notes: 'Looool'}})");
            session.WriteToDatabase($"MERGE (:Spawn:Successful {{ Name: 'SPN-GT-01-02', Type: 'Rye', Notes: 'Looool'}})");
            session.WriteToDatabase($"MERGE (:Spawn:Successful {{ Name: 'SPN-BP-01-01', Type: 'Millet', Notes: 'Looool'}})");
            session.WriteToDatabase($"MERGE (:Bulk:Successful {{ Name: 'BLK-CVG-GT-01-02', Notes: 'Looool'}})");
            session.WriteToDatabase($"MERGE (:Bulk:Successful {{ Name: 'BLK-CVG-BP-01-01', Notes: 'Looool'}})");
            session.WriteToDatabase($"MERGE (:Fruit:Successful {{ Name: 'FRT-GT-01-02', WetWeight: 18, DryWeight: 100}})");
            session.WriteToDatabase($"MERGE (:Fruit:Successful {{ Name: 'FRT-GT-01-02', WetWeight: 12, DryWeight: 88}})");
            session.WriteToDatabase($"MERGE (:Fruit:Successful {{ Name: 'FRT-BP-01-01', WetWeight: 7, DryWeight: 82}})");
            session.WriteToDatabase($"MERGE (:Fruit:Failed {{ Name: 'FRT-BP-01-01', WetWeight: 7, DryWeight: 82}})");
        }

        public static void CreateRelationships(this IAsyncSession session)
        {
            session.WriteToDatabase($"MATCH (a:Account), (u:User) MERGE (a)-[:HAS]->(u)");
            session.WriteToDatabase($"MATCH (r:IAMRole), (p:Permission) WHERE r.Name = 'Administrator' AND p.Type IN ['Create','Update','Delete','Read'] MERGE (r)-[:HAS]->(p)");
            session.WriteToDatabase($"MATCH (r:IAMRole), (p:Permission) WHERE r.Name = 'Reader' AND p.Type IN ['Read'] MERGE (r)-[:HAS]->(p)");
            session.WriteToDatabase($"MATCH (r:IAMRole), (p:Permission) WHERE r.Name = 'Writer' AND p.Type IN ['Update', 'Read', 'Create'] MERGE (r)-[:HAS]->(p)");
            session.WriteToDatabase($"MATCH (u:User {{ Name:'Ryan' }}), (r:IAMRole) WHERE r.Name = 'Administrator' MERGE (u)-[:HAS]->(r)");
            session.WriteToDatabase($"MATCH (u:User), (r:IAMRole) WHERE u.Name IN ['Calum','Aiden'] AND r.Name IN ['Reader','Writer'] MERGE (u)-[:HAS]->(r)");
            
            session.WriteToDatabase($"MATCH (c:Culture:SporePrint:Purchase), (v:Vendor) MERGE (c)-[:PURCHASED_FROM]->(v)");
            session.WriteToDatabase($"MATCH (c:Culture:SporePrint:Purchase), (d:Day {{ day: 1 }})<-[:HAS_DAY]-(:Month {{ month: 4 }})<-[:HAS_MONTH]-(y:Year {{ year: 2021 }}) MERGE (c)-[:PURCHASED_ON]->(d)");
            session.WriteToDatabase($"MATCH (p:Culture:SporePrint:Purchase), (c:Culture:Agar) WHERE c.Name IN ['SP-GT-01-01','SP-GT-01-02','SP-GT-01-03'] MERGE (c)-[:HAS_PARENT]->(p)");
            session.WriteToDatabase($"MATCH (p:Culture:SporePrint:Purchase), (d:Day {{ day: 2 }})<-[:HAS_DAY]-(:Month {{ month: 4 }})<-[:HAS_MONTH]-(y:Year {{ year: 2021 }}) MERGE (s)-[:CREATED_ON]->(d)");
            session.WriteToDatabase($"MATCH (p:Culture:SporePrint:Purchase), (s:Strain {{ Name: 'Golden Teacher' }}) MERGE (p)-[:IS_STRAIN]->(s)");
            
            session.WriteToDatabase($"MATCH (c:Culture:Agar), (d:Day {{ day: 5 }})<-[:HAS_DAY]-(:Month {{ month: 4 }})<-[:HAS_MONTH]-(y:Year {{ year: 2021 }}) WHERE c.Name IN ['SP-GT-01-01','SP-GT-01-02','SP-GT-01-03'] MERGE (c)-[:CREATED_ON]->(d)");   
            session.WriteToDatabase($"MATCH (c:Culture:Agar), (u:User {{ Name: 'Ryan' }}) WHERE c.Name IN ['SP-GT-01-01','SP-GT-01-02','SP-GT-01-03'] MERGE (u)-[:CREATED]->(c)");
            session.WriteToDatabase($"MATCH (c:Culture:Agar), (r:Recipe {{ Name: 'Blue Transparent Agar' }}) MERGE (c)-[:CREATED_USING]->(r)");
            session.WriteToDatabase($"MATCH (c:Culture:Agar), (l:Location {{ Name: 'Grow Tent' }}) MERGE (c)-[:STORED_IN]->(l)");
            session.WriteToDatabase($"MATCH (c:Culture:Agar), (s:Strain {{ Name: 'Golden Teacher' }}) WHERE c.Name IN ['SP-GT-01-01','SP-GT-01-02','SP-GT-01-03'] MERGE (c)-[:IS_STRAIN]->(s)");
            session.WriteToDatabase($"MATCH (c:Culture:Agar), (s:Strain {{ Name: 'B+' }}) WHERE c.Name IN ['SP-BP-01-01'] MERGE (c)-[:IS_STRAIN]->(s)");
            
            session.WriteToDatabase($"MATCH (p:Culture:SporePrint {{ Name: 'SP-BP-01' }}), (c:Culture:Agar) WHERE c.Name IN ['SP-BP-01-01'] MERGE (c)-[:HAS_PARENT]->(p)");
            
            session.WriteToDatabase($"MATCH (c:Culture:Agar), (d:Day {{ day: 5 }})<-[:HAS_DAY]-(:Month {{ month: 4 }})<-[:HAS_MONTH]-(y:Year {{ year: 2021 }}) WHERE c.Name IN ['SP-BP-01-01'] MERGE (c)-[:CREATED_ON]->(d)");
            session.WriteToDatabase($"MATCH (c:Culture:Agar), (u:User {{ Name: 'Ryan' }}) WHERE c.Name IN ['SP-BP-01-01'] MERGE (u)-[:CREATED]->(c)");
            
            session.WriteToDatabase($"MATCH (i:Ingredient), (r:Recipe {{ Name: 'Popcorn Spawn' }}) WHERE i.Name IN ['Popcorn', 'Gypsum'] MERGE (r)-[:CREATED_USING]->(i)");
            session.WriteToDatabase($"MATCH (i:Ingredient), (r:Recipe {{ Name: 'Blue Transparent Agar' }}) WHERE i.Name IN ['Popcorn', 'Gypsum'] MERGE (r)-[:CREATED_USING]->(i)");
             
            session.WriteToDatabase($"MATCH (s:Spawn:Successful {{ Name: 'SPN-GT-01-02' }}), (c:Culture:Agar {{ Name: 'SP-GT-01-02' }}) MERGE (s)-[:HAS_PARENT]->(c)");
            session.WriteToDatabase($"MATCH (s:Spawn:Successful {{ Name: 'SPN-GT-01-02' }}), (d:Day {{ day: 7 }})<-[:HAS_DAY]-(:Month {{ month: 4 }})<-[:HAS_MONTH]-(y:Year {{ year: 2021 }}) MERGE (s)-[:CREATED_ON]->(d)");
            session.WriteToDatabase($"MATCH (s:Spawn:Successful {{ Name: 'SPN-GT-01-02' }}), (u:User {{ Name: 'Ryan' }}) MERGE (u)-[:CREATED]->(s)");
            session.WriteToDatabase($"MATCH (s:Spawn:Successful {{ Name: 'SPN-GT-01-02' }}), (r:Recipe {{ Name: 'Popcorn Spawn' }}) MERGE (s)-[:CREATED_USING]->(r)");
            session.WriteToDatabase($"MATCH (sp:Spawn:Successful {{ Name: 'SPN-GT-01-02' }}), (s:Strain {{ Name: 'Golden Teacher' }})  MERGE (sp)-[:IS_STRAIN]->(s)");
            
            session.WriteToDatabase($"MATCH (s:Spawn:Successful {{ Name: 'SPN-BP-01-01' }}), (c:Culture:Agar {{ Name: 'SP-BP-01-01' }}) MERGE (s)-[:HAS_PARENT]->(c)");
            session.WriteToDatabase($"MATCH (s:Spawn:Successful {{ Name: 'SPN-BP-01-01' }}), (d:Day {{ day: 7 }})<-[:HAS_DAY]-(:Month {{ month: 4 }})<-[:HAS_MONTH]-(y:Year {{ year: 2021 }}) MERGE (s)-[:CREATED_ON]->(d)");
            session.WriteToDatabase($"MATCH (s:Spawn:Successful {{ Name: 'SPN-BP-01-01' }}), (u:User {{ Name: 'Ryan' }}) MERGE (u)-[:CREATED]->(s)");
            session.WriteToDatabase($"MATCH (s:Spawn:Successful {{ Name: 'SPN-BP-01-01' }}), (r:Recipe {{ Name: 'Popcorn Spawn' }}) MERGE (s)-[:CREATED_USING]->(r)");
            session.WriteToDatabase($"MATCH (sp:Spawn:Successful {{ Name: 'SPN-BP-01-01' }}), (s:Strain {{ Name: 'B+' }}) MERGE (sp)-[:IS_STRAIN]->(s)");

        }

        public static void CreateRolesAndPermissions(this IAsyncSession session)
        {
            var rolePermissions = new Dictionary<Roles, List<Permissions>>()
            {
                { Roles.Administrator, new List<Permissions> { Permissions.Delete, Permissions.Create, Permissions.Update, Permissions.Read }},
                { Roles.Reader,        new List<Permissions> { Permissions.Read }},
                { Roles.Writer,        new List<Permissions> { Permissions.Create, Permissions.Update, Permissions.Read }},
            };

            var rolePermissionFormatted = JsonConvert.SerializeObject(rolePermissions);

            foreach (var role in rolePermissions)
            {
                session.WriteToDatabase($"MERGE (:IAMRole {{ Name: '{role.Key}'}});");
            }


            foreach (var permission in Enum.GetValues(typeof(Permissions)))
            {
                session.WriteToDatabase($"MERGE (:Permission {{ Type: '{permission}' }})");
            }
        }

        public static void TestReads(this IAsyncSession session)
        {
            var query = @"MATCH (n:Permission) RETURN n.Type AS id";
            
            //var result = ReadFromDatabase(session, query);
            
           // foreach (var record in result)
          //  {
            //    var line = record["id"].As<string>();
           //     Console.WriteLine(line);
           // }
            
            Console.WriteLine(Guid.NewGuid());
        }
    }
}





