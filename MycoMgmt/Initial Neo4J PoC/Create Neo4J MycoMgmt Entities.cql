CREATE
    (
        :Vendor
        {
            Id:1,
            Name: 'MycoPunks',
            URL: 'https://mycopunks.com'
        }
    ),
    (   :Strain
        {
            Id:1,
            Name: 'Golden Teacher',
            Effects: [
                'Visuals',
                'Calming',
                'Oneness'
            ]
        }
    ),
    (
        :Culture
        :SporePrint
        :Purchase
        { 
            Id:1,
            Name:'SP-GT-01'
        }
    ),
    (
        :Culture
        :Agar
        { 
            Id:2,
            Name:'AG-GT-01'
        }
    ),
    (
        :Culture
        :Agar
        { 
            Id:3,
            Name:'AG-GT-02'
        }
    ),
    (
        :Culture
        :Agar
        { 
            Id:4,
            Name:'AG-GT-03'
        }
    ),
    (:Ingredient { Name: 'Popcorn' }), 
    (:Ingredient { Name: 'Gypsum' }), 
    (:Ingredient { Name: 'Agar' }), 
    (:Ingredient { Name: 'Food Colouring' }),
    (:Ingredient { Name: 'Vermiculite' }),
    (:Ingredient { Name: 'Coco-Coir' }),
    (
        :Recipe
        {
            Id: 1,
            Name: 'Popcorn Spawn',
            Type: 'Grain Spawn',
            Desc: 'Wicked spawn!',
            Steps: '1... 2... 3...'
        }
    ),
    (
        :Recipe
        {
            Id: 2,
            Name: 'Coco Coir Bulk',
            Type: 'Bulk',
            Desc: 'Wicked Coir!',
            Steps: '1... 2... 3...'
        }
    ),
    (
        :Recipe
        {
            Id: 3,
            Name: 'CVG Bulk',
            Type: 'Bulk',
            Desc: 'Wicked CVG!',
            Steps: '1... 2... 3...'
        }
    ),
    (
        :Recipe
        {
            Id: 4,
            Name: 'Blue Transparent Agar',
            Type: 'Agar',
            Desc: 'Mad Cool Agar..',
            Steps: '1... 2... 3...'
        }
    ),
    (
        :Spawn
        :Success
        { 
            Id:1,
            Name:'SP-PC-GT01-01',
            Type: 'Popcorn',
            Notes: 'PC for 45 mins!'
        }
    ),
    (
        :Spawn
        :Failure
        { 
            Id:2,
            Name:'SP-PC-GT01-02',
            Type: 'Popcorn',
            Notes: 'PC for 45 mins!'
        }
    ),
    (
        :Bulk
        :Success
        { 
            Id:1,
            Name:'BLK-PC-GT01-01',
            Notes: 'PC for 45 mins!'
        }
    ),
    (
        :Bulk
        :Success
        { 
            Id:2,
            Name:'BLK-PC-GT01-02',
            Notes: 'PC for 45 mins!'
        }
    ),
    (
        :Fruit
        :Success
        { 
            Id:1,
            Name:'FRT-SP-PC-01',
            WetWeight: 100,
            DryWeight: 11
        }
    ),
    (
        :Fruit
        :Success
        { 
            Id:2,
            Name:'FRT-SP-PC-02',
            WetWeight: 212,
            DryWeight: 19
        }
    ),
    (
        :Fruit
        :Failure
        { 
            Id:3,
            Name:'FRT-SP-PC-02'
        }
    ),
    (
        :Location
        { 
            Id:1,
            Name:'Incubation Chamber'
        }
    ),
    (
        :Location
        { 
            Id:2,
            Name:'Fruiting Chamber'
        }
    ),
    (
        :Account
        {
            Id: 1,
            Name: 'RG Ltd.'
        }
    ),
    (
        :User
        {
            Name: 'Ryan'
        }
    ),
    (
        :User
        {
            Name: 'Calum'
        }
    ),
    (
        :IAMRole
        {
            Name: 'Administrator'
        }
    ),
    (
        :IAMRole
        {
            Name: 'Writer'
        }
    ),
    (
        :IAMRole
        {
            Name: 'Reader'
        }
    ),
    (
        :Permission
        {
            Type: 'Delete'
        }
    ),
    (
        :Permission
        {
            Type: 'Update'
        }
    ),
    (
        :Permission
        {
            Type: 'Create'
        }
    ),
    (
        :Permission
        {
            Type: 'Read'
        }
    );

// 1st Level
MATCH (c:Culture {Id:1}), (v:Vendor {Id:1}) MERGE (c)-[r:PURCHASED_FROM { PurchasedOn: '2022-10-01T23:10:00' }]->(v); 

// 2nd Level
MATCH (c:Culture), (p:Culture) WHERE c.Id IN [2,3,4] AND p.Id = 1 MERGE (c)<-[r:HAS_CHILD { CreatedOn: '2022-10-02T13:00:00' }]-(p);

// 3rd Level
MATCH (s:Spawn), (c:Culture) WHERE s.Id IN [1,2] AND c.Id = 2 MERGE (s)<-[:HAS_CHILD { CreatedOn: '2022-10-03T11:00:00' }]-(c);

// 4th  
MATCH (b:Bulk), (s:Spawn:Success) MERGE (b)<-[:HAS_CHILD { CreatedOn: '2022-10-03T11:00:00' }]-(s);

// 5th Level
MATCH (f:Fruit), (b:Bulk) WHERE b.Id = 1 AND f.Id IN [1,2,3] MERGE (f)<-[:HAS_CHILD { CreatedOn: '2022-10-03T11:00:00' }]-(b);



// Entity > Strain
MATCH (c:Culture), (s:Strain) WHERE s.Id = 1 MERGE (c)-[r:IS_STRAIN]->(s);
MATCH (c:Spawn),   (s:Strain) WHERE s.Id = 1 MERGE (c)-[r:IS_STRAIN]->(s);
MATCH (c:Bulk),    (s:Strain) WHERE s.Id = 1 MERGE (c)-[r:IS_STRAIN]->(s);
MATCH (c:Fruit),   (s:Strain) WHERE s.Id = 1 MERGE (c)-[r:IS_STRAIN]->(s);

// Entity > Location
MATCH (a:Agar), (l:Location) WHERE a.Id IN [2,3,4] AND l.Id = 1 MERGE (a)-[r:STORED_IN]->(l);
MATCH (b:Bulk), (l:Location) WHERE l.Id = 2 MERGE (b)-[r:STORED_IN]->(l);

// Entity > Recipe
MATCH (s:Spawn), (r:Recipe) WHERE s.Id IN [1,2] AND r.Id = 1 MERGE (s)-[:CREATED_USING]->(r);
MATCH (b:Bulk),  (r:Recipe) WHERE b.Id = 1 AND r.Name = 'Coco Coir Bulk' MERGE (b)-[:CREATED_USING]->(r);
MATCH (b:Bulk),  (r:Recipe) WHERE b.Id = 2 AND r.Name = 'CVG Bulk' MERGE (b)-[:CREATED_USING]->(r);

// Recipe > Ingredient
MATCH (i:Ingredient), (r:Recipe) WHERE i.Name IN ['Popcorn','Gypsum']      AND r.Id = 1 MERGE (r)-[:CREATED_USING]->(i);
MATCH (i:Ingredient), (r:Recipe) WHERE i.Name IN ['Food Colouring','Agar'] AND r.Id = 4 MERGE (r)-[:CREATED_USING]->(i);
MATCH (i:Ingredient), (r:Recipe) WHERE i.Name IN ['Coco-Coir'] AND r.Name = 'Coco Coir Bulk' MERGE (r)-[:CREATED_USING]->(i);
MATCH (i:Ingredient), (r:Recipe) WHERE i.Name IN ['Coco-Coir','Vermiculite','Gypsum'] AND r.Name = 'CVG Bulk' MERGE (r)-[:CREATED_USING]->(i);

// Map Innoculation Date
MATCH
    (p:Purchase),
    (d:Day {day: 28})<-[:HAS_DAY]-(:Month {month:7})<-[:HAS_MONTH]-(:Year {year:2022})
WHERE
    p.Id = 1
MERGE (p)-[r:WAS_PURCHASED_ON]->(d)
RETURN *;

MATCH\
    (a:Agar),
    (d:Day {day: 29})<-[:HAS_DAY]-(:Month {month:7})<-[:HAS_MONTH]-(:Year {year:2022})
MERGE (a)-[r:WAS_INNOCULATED_ON]->(d)
RETURN *;

MATCH
    (a:Spawn),
    (d:Day {day: 6})<-[:HAS_DAY]-(:Month {month:8})<-[:HAS_MONTH]-(:Year {year:2022})
MERGE (a)-[r:WAS_INNOCULATED_ON]->(d)
RETURN *;

MATCH
    (a:Bulk),
    (d:Day {day: 12})<-[:HAS_DAY]-(:Month {month:8})<-[:HAS_MONTH]-(:Year {year:2022})
MERGE (a)-[r:WAS_INNOCULATED_ON]->(d)
RETURN *;

MATCH
    (a:Fruit),
    (d:Day {day: 21})<-[:HAS_DAY]-(:Month {month:8})<-[:HAS_MONTH]-(:Year {year:2022})
MERGE (a)-[r:WAS_INNOCULATED_ON]->(d)
RETURN *;

// Create Account > User
MATCH (a:Account), (u:User) MERGE (a)-[:HAS]->(u);

// Create User > Role
MATCH (u:User {Name:'Ryan'}), (r:IAMRole) WHERE r.Name = 'Administrator' MERGE (u)-[:HAS]->(r);
MATCH (u:User {Name:'Calum'}), (r:IAMRole) WHERE r.Name IN ['Writer','Reader'] MERGE (u)-[:HAS]->(r);


// Create Role > Permission
MATCH (i:IAMRole {Name:'Administrator'}), (p:Permission) WHERE p.Type IN ['Delete','Update','Create','Read'] MERGE (i)-[:HAS]->(p);
MATCH (i:IAMRole {Name:'Writer'}),        (p:Permission) WHERE p.Type IN ['Update','Create'] MERGE (i)-[:HAS]->(p);
MATCH (i:IAMRole {Name:'Reader'}),        (p:Permission) WHERE p.Type IN ['Read'] MERGE (i)-[:HAS]->(p);


MATCH (u:User {Name:'Ryan'}), (c:Culture {Name:'AG-GT-01'}) MERGE (u)-[:CREATED]->(c);
MATCH (u:User {Name:'Ryan'}), (c:Culture {Name:'AG-GT-01'}) MERGE (u)-[:MODIFIED]->(c);
MATCH (c:Culture {Name:'AG-GT-01'}),(d:Day {day: 29})<-[:HAS_DAY]-(:Month {month:7})<-[:HAS_MONTH]-(:Year {year:2022}) MERGE (c)-[:CREATED_ON]->(d);
MATCH (c:Culture {Name:'AG-GT-01'}),(d:Day {day: 29})<-[:HAS_DAY]-(:Month {month:7})<-[:HAS_MONTH]-(:Year {year:2022}) MERGE (c)-[:MODIFIED_ON]->(d);







// Create DateTime Tree / Index
WITH range(2021, 2024) AS years, range(1,12) as months
FOREACH(year IN years |
    CREATE (y:Year {year: year})
    FOREACH(month IN months |
        CREATE (m:Month {month: month})
        CREATE (y)-[:HAS_MONTH]->(m)
        FOREACH(day IN (CASE
                            WHEN month IN [1,3,5,7,8,10,12] THEN range(1,31)
                            WHEN month = 2 THEN CASE
                                                    WHEN year % 4 <> 0 THEN range(1,28)
                                                    WHEN year % 100 <> 0 THEN range(1,29)
                                                    WHEN year % 400 = 0 THEN range(1,29)
                                                    ELSE range(1,28)
                                                END
                                            ELSE 
                                                range(1,30)
                        END) |
            CREATE (d:Day {day: day})
            CREATE (m)-[:HAS_DAY]->(d))))

WITH *

MATCH (year:Year)-[:HAS_MONTH]->(month)-[:HAS_DAY]->(day)
WITH year,month,day
ORDER BY year.year, month.month, day.day
WITH collect(day) as days
FOREACH(i in RANGE(0, size(days)-2) |
    FOREACH(day1 in [days[i]] |
        FOREACH(day2 in [days[i+1]] |
            CREATE (day1)-[:NEXT]->(day2))))



            /*
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MycoMgmt.Populator.Models;
using MycoMgmt.Populator;
using Neo4j.Driver;

namespace MycoMgmt.Populator
{
    public class Neo4JPopulator
    {
        public readonly IDriver _driver; 
        

        
        public static void Main(string[] args)
        {
            using var _client = new Neo4J("neo4j://localhost:7687", "test", "test");
            
            _client.PrintGreeting("Hello!");
            
            /*
             var rolePermissions = new Dictionary<Roles, List<Permissions>>()
            {
                {
                    Roles.Administrator,
                    new List<Permissions>
                        { Permissions.Delete, Permissions.Create, Permissions.Update, Permissions.Read }
                },
                { Roles.Reader, new List<Permissions> { Permissions.Read } },
                { Roles.Writer, new List<Permissions> { Permissions.Create, Permissions.Update, Permissions.Read } },
            };

            var vendor = new Vendor
            {
                Name = "Myco Punks",
                URL = new Uri("https://mycopunks.com")
            };

            var agarRecipe = new Recipe
            {
                Name = "BlueTransparentAgar",
                Type = RecipeTypes.Agar,
                Description = "Maddddd agar",
                Steps = new List<string>
                {
                    "Grab some agar mix",
                    "Mix this with sugar water",
                    "Boil for 1 hour",
                    "Pressure Cook at 45PSI",
                    "4 Hours, all good"
                }
            };

            var spawnRecipe = new Recipe
            {
                Name = "PopcornRecipe",
                Type = RecipeTypes.Spawn,
                Description = "Maddddd spawn",
                Steps = new List<string>
                {
                    "Grab some agar mix",
                    "Mix this with sugar water",
                    "Boil for 1 hour",
                    "Pressure Cook at 45PSI",
                    "4 Hours, all good"
                }
            };

            var account = new Account
            {
                Name = "RG Ltd.",
                Users = new List<Users> { Users.Aiden, Users.Callum }
            };

            var users = new Dictionary<Users, Roles>()
            {
                { Users.Aiden, Roles.Reader },
                { Users.Calum, Roles.Reader },
                { Users.Callum, Roles.Writer },
                { Users.Ryan, Roles.Administrator }
            };

            var cultures = new List<Culture>
            {
                new Culture
                {
                    Name = "SS-GT-01",
                    Strain = Strains.GoldenTeacher,
                    Location = Locations.IncubationChamber,
                    Parent = account.Id,
                    Vendor = vendor,
                    Created = new Dictionary<DateTime, Users> { { DateTime.Now.AddDays(-7), Users.Ryan } },
                    Modified = new Dictionary<DateTime, Users> { { DateTime.Now.AddDays(-6), Users.Callum } },
                    Tags = new List<EntityTypes>
                    {
                        EntityTypes.Culture,
                        EntityTypes.Purchase,
                        EntityTypes.SporeSyringe
                    }
                },
                new Culture
                {
                    Name = "SP-BP-01",
                    Strain = Strains.BPlus,
                    Location = Locations.IncubationChamber,
                    Parent = account.Id,
                    Vendor = vendor,
                    Created = new Dictionary<DateTime, Users> { { DateTime.Now.AddDays(-5), Users.Ryan } },
                    Tags = new List<EntityTypes>
                    {
                        EntityTypes.Culture,
                        EntityTypes.Purchase,
                        EntityTypes.SporePrint
                    }
                }
            };

            var cultures2 = new List<Culture>
            {
                new Culture
                {
                    Name = "AG-GT-01-01",
                    Strain = Strains.GoldenTeacher,
                    Location = Locations.IncubationChamber,
                    Parent = cultures[0].Id,
                    Recipe = "BlueTransparentAgar",
                    Created = new Dictionary<DateTime, Users> { { DateTime.Now.AddDays(-6), Users.Ryan } },
                    Tags = new List<EntityTypes>
                    {
                        EntityTypes.Culture,
                        EntityTypes.Agar
                    }
                },
                new Culture
                {
                    Name = "AG-GT-01-02",
                    Strain = Strains.GoldenTeacher,
                    Location = Locations.IncubationChamber,
                    Parent = cultures[0].Id,
                    Recipe = "BlueTransparentAgar",
                    Created = new Dictionary<DateTime, Users> { { DateTime.Now.AddDays(-6), Users.Ryan } },
                    Tags = new List<EntityTypes>
                    {
                        EntityTypes.Culture,
                        EntityTypes.Agar
                    }
                },
                new Culture
                {
                    Name = "AG-GT-01-03",
                    Strain = Strains.GoldenTeacher,
                    Location = Locations.IncubationChamber,
                    Parent = cultures[0].Id,
                    Recipe = "BlueTransparentAgar",
                    Created = new Dictionary<DateTime, Users> { { DateTime.Now.AddDays(-6), Users.Ryan } },
                    Tags = new List<EntityTypes>
                    {
                        EntityTypes.Culture,
                        EntityTypes.Agar
                    }
                },
                new Culture
                {
                    Name = "AG-BP-01-01",
                    Strain = Strains.BPlus,
                    Location = Locations.IncubationChamber,
                    Parent = cultures[1].Id,
                    Recipe = "BlueTransparentAgar",
                    Created = new Dictionary<DateTime, Users> { { DateTime.Now.AddDays(-6), Users.Ryan } },
                    Tags = new List<EntityTypes>
                    {
                        EntityTypes.Culture,
                        EntityTypes.Agar
                    }
                },
                new Culture
                {
                    Name = "AG-BP-01-02",
                    Strain = Strains.BPlus,
                    Location = Locations.IncubationChamber,
                    Parent = cultures[1].Id,
                    Recipe = "BlueTransparentAgar",
                    Created = new Dictionary<DateTime, Users> { { DateTime.Now.AddDays(-6), Users.Ryan } },
                    Tags = new List<EntityTypes>
                    {
                        EntityTypes.Culture,
                        EntityTypes.Agar
                    }
                }
            };

            var spawn = new List<Spawn>
            {
                new Spawn
                {
                    Name = "SP-GT-01-01",
                    Type = SpawnTypes.PopCorn,
                    Notes = "PC for 45 mins",
                    Recipe = "PopcornRecipe",
                    Location = Locations.IncubationChamber,
                    Parent = cultures2[0].Id,
                    Successful = true,
                    Created = new Dictionary<DateTime, Users> { { DateTime.Now.AddDays(-5), Users.Callum } },
                    Modified = new Dictionary<DateTime, Users> { { DateTime.Now.AddHours(-4), Users.Ryan } },
                    Tags = new List<EntityTypes> { EntityTypes.Spawn, EntityTypes.Success }
                },
                new Spawn
                {
                    Name = "SP-GT-01-02",
                    Type = SpawnTypes.PopCorn,
                    Notes = "PC for 45 mins",
                    Recipe = "PopcornRecipe",
                    Location = Locations.IncubationChamber,
                    Parent = cultures2[0].Id,
                    Successful = true,
                    Created = new Dictionary<DateTime, Users> { { DateTime.Now.AddDays(-5), Users.Callum } },
                    Modified = new Dictionary<DateTime, Users> { { DateTime.Now.AddHours(-4), Users.Ryan } },
                    Tags = new List<EntityTypes> { EntityTypes.Spawn, EntityTypes.Success }
                },
                new Spawn
                {
                    Name = "SP-GT-01-03",
                    Type = SpawnTypes.PopCorn,
                    Notes = "PC for 45 mins",
                    Recipe = "PopcornRecipe",
                    Location = Locations.IncubationChamber,
                    Parent = cultures2[0].Id,
                    Created = new Dictionary<DateTime, Users> { { DateTime.Now.AddDays(-5), Users.Callum } },
                    Modified = new Dictionary<DateTime, Users> { { DateTime.Now.AddHours(-4), Users.Ryan } },
                    Tags = new List<EntityTypes> { EntityTypes.Spawn }
                },
                new Spawn
                {
                    Name = "SP-GT-01-04",
                    Type = SpawnTypes.PopCorn,
                    Notes = "PC for 45 mins",
                    Recipe = "PopcornRecipe",
                    Location = Locations.IncubationChamber,
                    Parent = cultures2[0].Id,
                    Created = new Dictionary<DateTime, Users> { { DateTime.Now.AddDays(-5), Users.Callum } },
                    Modified = new Dictionary<DateTime, Users> { { DateTime.Now.AddHours(-4), Users.Ryan } },
                    Tags = new List<EntityTypes> { EntityTypes.Spawn }
                },
                new Spawn
                {
                    Name = "SP-BP-01-01",
                    Type = SpawnTypes.PopCorn,
                    Notes = "PC for 45 mins",
                    Recipe = "PopcornRecipe",
                    Location = Locations.IncubationChamber,
                    Parent = cultures2[1].Id,
                    Successful = true,
                    Created = new Dictionary<DateTime, Users> { { DateTime.Now.AddDays(-5), Users.Callum } },
                    Modified = new Dictionary<DateTime, Users> { { DateTime.Now.AddHours(-4), Users.Ryan } },
                    Tags = new List<EntityTypes> { EntityTypes.Spawn, EntityTypes.Success }
                },
                new Spawn
                {
                    Name = "SP-BP-01-02",
                    Type = SpawnTypes.PopCorn,
                    Notes = "PC for 45 mins",
                    Recipe = "PopcornRecipe",
                    Location = Locations.IncubationChamber,
                    Parent = cultures2[1].Id,
                    Created = new Dictionary<DateTime, Users> { { DateTime.Now.AddDays(-5), Users.Callum } },
                    Modified = new Dictionary<DateTime, Users> { { DateTime.Now.AddHours(-4), Users.Ryan } },
                    Tags = new List<EntityTypes> { EntityTypes.Spawn }
                },
                new Spawn
                {
                    Name = "SP-BP-01-03",
                    Type = SpawnTypes.PopCorn,
                    Notes = "PC for 45 mins",
                    Recipe = "PopcornRecipe",
                    Location = Locations.IncubationChamber,
                    Parent = cultures2[1].Id,
                    Created = new Dictionary<DateTime, Users> { { DateTime.Now.AddDays(-5), Users.Callum } },
                    Modified = new Dictionary<DateTime, Users> { { DateTime.Now.AddHours(-4), Users.Ryan } },
                    Tags = new List<EntityTypes> { EntityTypes.Spawn }
                },
                new Spawn
                {
                    Name = "SP-BP-02-01",
                    Type = SpawnTypes.PopCorn,
                    Notes = "PC for 45 mins",
                    Recipe = "PopcornRecipe",
                    Location = Locations.IncubationChamber,
                    Parent = cultures2[4].Id,
                    Successful = true,
                    Created = new Dictionary<DateTime, Users> { { DateTime.Now.AddDays(-4), Users.Callum } },
                    Modified = new Dictionary<DateTime, Users> { { DateTime.Now.AddHours(-3), Users.Ryan } },
                    Tags = new List<EntityTypes> { EntityTypes.Spawn, EntityTypes.Success }
                },
                new Spawn
                {
                    Name = "SP-BP-02-02",
                    Type = SpawnTypes.PopCorn,
                    Notes = "PC for 45 mins",
                    Recipe = "PopcornRecipe",
                    Location = Locations.IncubationChamber,
                    Parent = cultures2[4].Id,
                    Successful = true,
                    Created = new Dictionary<DateTime, Users> { { DateTime.Now.AddDays(-4), Users.Callum } },
                    Tags = new List<EntityTypes> { EntityTypes.Spawn, EntityTypes.Success }
                }
            };
         */   
        }
    }
}
*/




