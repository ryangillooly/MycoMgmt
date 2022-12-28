// Create DateTime Tree / Index
WITH range(2021, 2024) AS years, range(1,12) as months
FOREACH(year IN years |
    CREATE (y:Year {year: year})
    FOREACH(month IN months |
        CREATE (m:Month {month: month})
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
            CREATE (day1)-[:NEXT]->(day2))));


                CREATE CONSTRAINT UniqueName_Culture    IF NOT EXISTS FOR (c:Culture)    REQUIRE c.Name IS UNIQUE;
                CREATE CONSTRAINT UniqueName_Bulk       IF NOT EXISTS FOR (c:Bulk)       REQUIRE c.Name IS UNIQUE;
                CREATE CONSTRAINT UniqueName_Spawn      IF NOT EXISTS FOR (c:Spawn)      REQUIRE c.Name IS UNIQUE;
                CREATE CONSTRAINT UniqueName_Fruit      IF NOT EXISTS FOR (c:Fruit)      REQUIRE c.Name IS UNIQUE;
                CREATE CONSTRAINT UniqueName_Strain     IF NOT EXISTS FOR (c:Strain)     REQUIRE c.Name IS UNIQUE;
                CREATE CONSTRAINT UniqueName_Location   IF NOT EXISTS FOR (c:Location)   REQUIRE c.Name IS UNIQUE;
                CREATE CONSTRAINT UniqueName_Recipe     IF NOT EXISTS FOR (c:Recipe)     REQUIRE c.Name IS UNIQUE;
                CREATE CONSTRAINT UniqueName_Ingredient IF NOT EXISTS FOR (c:Ingredient) REQUIRE c.Name IS UNIQUE;
                CREATE CONSTRAINT UniqueName_User       IF NOT EXISTS FOR (c:User)       REQUIRE c.Name IS UNIQUE;
                CREATE CONSTRAINT UniqueName_Account    IF NOT EXISTS FOR (c:Account)    REQUIRE c.Name IS UNIQUE;
                CREATE CONSTRAINT UniqueName_Permission IF NOT EXISTS FOR (c:Permission) REQUIRE c.Name IS UNIQUE;
                CREATE CONSTRAINT UniqueName_Role       IF NOT EXISTS FOR (c:IAMRole)    REQUIRE c.Name IS UNIQUE;
                CREATE CONSTRAINT UniqueName_Vendor     IF NOT EXISTS FOR (c:Vendor)     REQUIRE c.Name IS UNIQUE;
                                
                MERGE (:Strain { Name: 'GoldenTeacher', Effects:['Visuals','Calming','Oneness']});
                MERGE (:Strain { Name: 'BPlus', Effects: ['Visuals','Calming','Oneness']});
                MERGE (:Strain { Name: 'Mazapatec', Effects: ['Visuals','Calming','Oneness']});
                
                MERGE (:Location {  Name: 'GrowTent', AgentConfigured: 'false' });
                MERGE (:Location {  Name: 'IncubationChamber', AgentConfigured: 'true' });
                MERGE (:Location {  Name: 'FruitingChamber', AgentConfigured: 'false' });
              
              
              MERGE (:Account {  Name: 'RG Ltd.'});
              MERGE (:Vendor {  Name: 'MycoPunks', Url: 'https://mycopunks.com'});
              MERGE (:User {  Name: 'Ryan' });
              MERGE (:User {  Name: 'Calum' });
              MERGE (:User {  Name: 'Aiden' });
              MERGE (:Recipe {  Name: 'Popcorn Spawn', Type: 'Grain Spawn', Description: 'Madddddd', Steps: '1.... 2.... 3....' });
              MERGE (:Recipe {  Name: 'Coco Coir Bulk', Type: 'Bulk', Description: 'Madddddd', Steps: '1.... 2.... 3....' });
              MERGE (:Recipe {  Name: 'CVG Bulk', Type: 'Bulk', Description: 'Madddddd', Steps: '1.... 2.... 3....' });
              MERGE (:Recipe {  Name: 'Blue Transparent Agar', Type: 'Agar', Description: 'Madddddd', Steps: '1.... 2.... 3....' });
              MERGE (:Ingredient { Name: 'Popcorn' });
              MERGE (:Ingredient { Name: 'Gypsum' });
              MERGE (:Ingredient { Name: 'Agar' });
              MERGE (:Ingredient { Name: 'Food Colouring' });
              MERGE (:Ingredient { Name: 'Vermiculite' });
              MERGE (:Ingredient { Name: 'Coco-Coir' });
              MERGE (:Culture:SporePrint:Purchase { Name: 'SP-GT-01'});
              MERGE (:Culture:SporePrint { Name: 'SP-BP-01'});
              MERGE (:Culture:Agar { Name: 'SP-GT-01-01'});
              MERGE (:Culture:Agar { Name: 'SP-GT-01-02'});
              MERGE (:Culture:Agar { Name: 'SP-GT-01-03'});
              MERGE (:Culture:Agar { Name: 'SP-BP-01-01'});
              MERGE (:Spawn:Failed { Name: 'SPN-GT-01-01', Type: 'Popcorn', Notes: 'Looool'});
              MERGE (:Spawn:Successful { Name: 'SPN-GT-01-02', Type: 'Rye', Notes: 'Looool'});
              MERGE (:Spawn:Successful { Name: 'SPN-BP-01-01', Type: 'Millet', Notes: 'Looool'});
              MERGE (:Bulk:Successful { Name: 'BLK-CVG-GT-01-02', Notes: 'Looool'});
              MERGE (:Bulk:Successful { Name: 'BLK-CVG-BP-01-01', Notes: 'Looool'});
              MERGE (:Fruit:Successful { Name: 'FRT-GT-01-02', WetWeight: 18, DryWeight: 100});
              MERGE (:Fruit:Successful { Name: 'FRT-GT-01-02', WetWeight: 12, DryWeight: 88});
              MERGE (:Fruit:Successful { Name: 'FRT-BP-01-01', WetWeight: 7, DryWeight: 82});
              MERGE (:Fruit:Failed { Name: 'FRT-BP-01-01', WetWeight: 7, DryWeight: 82});
              
              
              
              
              MATCH (a:Account), (u:User) MERGE (a)-[:HAS]->(u);
              MATCH (r:IAMRole), (p:Permission) WHERE r.Name = 'Administrator' AND p.Type IN ['Create','Update','Delete','Read'] MERGE (r)-[:HAS]->(p);
              MATCH (r:IAMRole), (p:Permission) WHERE r.Name = 'Reader' AND p.Type IN ['Read'] MERGE (r)-[:HAS]->(p);
              MATCH (r:IAMRole), (p:Permission) WHERE r.Name = 'Writer' AND p.Type IN ['Update', 'Read', 'Create'] MERGE (r)-[:HAS]->(p);
              MATCH (u:User { Name:'Ryan' }), (r:IAMRole) WHERE r.Name = 'Administrator' MERGE (u)-[:HAS]->(r);
              MATCH (u:User), (r:IAMRole) WHERE u.Name IN ['Calum','Aiden'] AND r.Name IN ['Reader','Writer'] MERGE (u)-[:HAS]->(r);

              MATCH (c:Culture:SporePrint:Purchase), (v:Vendor) MERGE (c)-[:PURCHASED_FROM]->(v);
              MATCH (c:Culture:SporePrint:Purchase), (d:Day { day: 1 })<-[:HAS_DAY]-(:Month { month: 4 })<-[:HAS_MONTH]-(y:Year { year: 2021 }) MERGE (c)-[:PURCHASED_ON]->(d);
              MATCH (p:Culture:SporePrint:Purchase), (c:Culture:Agar) WHERE c.Name IN ['SP-GT-01-01','SP-GT-01-02','SP-GT-01-03'] MERGE (c)-[:HAS_PARENT]->(p);
              MATCH (p:Culture:SporePrint:Purchase), (d:Day { day: 2 })<-[:HAS_DAY]-(:Month { month: 4 })<-[:HAS_MONTH]-(y:Year { year: 2021 }) MERGE (s)-[:CREATED_ON]->(d);
              MATCH (p:Culture:SporePrint:Purchase), (s:Strain { Name: 'Golden Teacher' }) MERGE (p)-[:IS_STRAIN]->(s);

              MATCH (c:Culture:Agar), (d:Day { day: 5 })<-[:HAS_DAY]-(:Month { month: 4 })<-[:HAS_MONTH]-(y:Year { year: 2021 }) WHERE c.Name IN ['SP-GT-01-01','SP-GT-01-02','SP-GT-01-03'] MERGE (c)-[:CREATED_ON]->(d);
              MATCH (c:Culture:Agar), (u:User { Name: 'Ryan' }) WHERE c.Name IN ['SP-GT-01-01','SP-GT-01-02','SP-GT-01-03'] MERGE (u)-[:CREATED]->(c);
              MATCH (c:Culture:Agar), (r:Recipe { Name: 'Blue Transparent Agar' }) MERGE (c)-[:CREATED_USING]->(r);
              MATCH (c:Culture:Agar), (l:Location { Name: 'Grow Tent' }) MERGE (c)-[:STORED_IN]->(l);
              MATCH (c:Culture:Agar), (s:Strain { Name: 'Golden Teacher' }) WHERE c.Name IN ['SP-GT-01-01','SP-GT-01-02','SP-GT-01-03'] MERGE (c)-[:IS_STRAIN]->(s);
              MATCH (c:Culture:Agar), (s:Strain { Name: 'B+' }) WHERE c.Name IN ['SP-BP-01-01'] MERGE (c)-[:IS_STRAIN]->(s);

              MATCH (p:Culture:SporePrint { Name: 'SP-BP-01' }), (c:Culture:Agar) WHERE c.Name IN ['SP-BP-01-01'] MERGE (c)-[:HAS_PARENT]->(p);

              MATCH (c:Culture:Agar), (d:Day { day: 5 })<-[:HAS_DAY]-(:Month { month: 4 })<-[:HAS_MONTH]-(y:Year { year: 2021 }) WHERE c.Name IN ['SP-BP-01-01'] MERGE (c)-[:CREATED_ON]->(d);
              MATCH (c:Culture:Agar), (u:User { Name: 'Ryan' }) WHERE c.Name IN ['SP-BP-01-01'] MERGE (u)-[:CREATED]->(c);

              MATCH (i:Ingredient), (r:Recipe { Name: 'Popcorn Spawn' }) WHERE i.Name IN ['Popcorn', 'Gypsum'] MERGE (r)-[:CREATED_USING]->(i);
              MATCH (i:Ingredient), (r:Recipe { Name: 'Blue Transparent Agar' }) WHERE i.Name IN ['Popcorn', 'Gypsum'] MERGE (r)-[:CREATED_USING]->(i);

              MATCH (s:Spawn:Successful { Name: 'SPN-GT-01-02' }), (c:Culture:Agar { Name: 'SP-GT-01-02' }) MERGE (s)-[:HAS_PARENT]->(c);
              MATCH (s:Spawn:Successful { Name: 'SPN-GT-01-02' }), (d:Day { day: 7 })<-[:HAS_DAY]-(:Month { month: 4 })<-[:HAS_MONTH]-(y:Year { year: 2021 }) MERGE (s)-[:CREATED_ON]->(d);
              MATCH (s:Spawn:Successful { Name: 'SPN-GT-01-02' }), (u:User { Name: 'Ryan' }) MERGE (u)-[:CREATED]->(s);
              MATCH (s:Spawn:Successful { Name: 'SPN-GT-01-02' }), (r:Recipe { Name: 'Popcorn Spawn' }) MERGE (s)-[:CREATED_USING]->(r);
              MATCH (sp:Spawn:Successful { Name: 'SPN-GT-01-02' }), (s:Strain { Name: 'Golden Teacher' })  MERGE (sp)-[:IS_STRAIN]->(s);

              MATCH (s:Spawn:Successful { Name: 'SPN-BP-01-01' }), (c:Culture:Agar { Name: 'SP-BP-01-01' }) MERGE (s)-[:HAS_PARENT]->(c);
              MATCH (s:Spawn:Successful { Name: 'SPN-BP-01-01' }), (d:Day { day: 7 })<-[:HAS_DAY]-(:Month { month: 4 })<-[:HAS_MONTH]-(y:Year { year: 2021 }) MERGE (s)-[:CREATED_ON]->(d);
              MATCH (s:Spawn:Successful { Name: 'SPN-BP-01-01' }), (u:User { Name: 'Ryan' }) MERGE (u)-[:CREATED]->(s);
              MATCH (s:Spawn:Successful { Name: 'SPN-BP-01-01' }), (r:Recipe { Name: 'Popcorn Spawn' }) MERGE (s)-[:CREATED_USING]->(r);
              MATCH (sp:Spawn:Successful { Name: 'SPN-BP-01-01' }), (s:Strain { Name: 'B+' }) MERGE (sp)-[:IS_STRAIN]->(s);

              
               CREATE CONSTRAINT UniqueName_Culture IF NOT EXISTS FOR (c:Culture) REQUIRE c.Name IS UNIQUE;
         
