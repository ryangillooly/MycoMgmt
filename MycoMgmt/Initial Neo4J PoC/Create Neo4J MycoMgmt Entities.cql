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
    );




// 1st Level
MATCH (c:Culture {Id:1}), (v:Vendor {Id:1}) CREATE (c)-[r:PURCHASED_FROM { PurchasedOn: '2022-10-01T23:10:00' }]->(v); 

// 2nd Level
MATCH (c:Culture), (p:Culture) WHERE c.Id IN [2,3,4] AND p.Id = 1 CREATE (c)-[r:HAS_PARENT { CreatedOn: '2022-10-02T13:00:00' }]->(p);

// 3rd Level
MATCH (s:Spawn), (c:Culture) WHERE s.Id IN [1,2] AND c.Id = 2 CREATE (s)-[:HAS_PARENT { CreatedOn: '2022-10-03T11:00:00' }]->(c);

// 4th  
MATCH (b:Bulk), (s:Spawn:Success) CREATE (s)-[:HAS_PARENT { CreatedOn: '2022-10-03T11:00:00' }]->(b);

// 5th Level
MATCH (f:Fruit), (b:Bulk) WHERE b.Id = 1 AND f.Id IN [1,2,3] CREATE (f)-[:HAS_PARENT { CreatedOn: '2022-10-03T11:00:00' }]->(b);



// Entity > Strain
MATCH (c:Culture), (s:Strain) WHERE s.Id = 1 CREATE (c)-[r:IS_STRAIN]->(s);
MATCH (c:Spawn),   (s:Strain) WHERE s.Id = 1 CREATE (c)-[r:IS_STRAIN]->(s);
MATCH (c:Bulk),    (s:Strain) WHERE s.Id = 1 CREATE (c)-[r:IS_STRAIN]->(s);
MATCH (c:Fruit),   (s:Strain) WHERE s.Id = 1 CREATE (c)-[r:IS_STRAIN]->(s);

// Entity > Location
MATCH (a:Agar), (l:Location) WHERE a.Id IN [2,3,4] AND l.Id = 1 CREATE (a)-[r:STORED_IN]->(l);
MATCH (b:Bulk), (l:Location) WHERE l.Id = 2 CREATE (b)-[r:STORED_IN]->(l);

// Entity > Recipe
MATCH (s:Spawn), (r:Recipe) WHERE s.Id IN [1,2] AND r.Id = 1 CREATE (s)-[:CREATED_USING]->(r);
MATCH (b:Bulk),  (r:Recipe) WHERE b.Id = 1 AND r.Name = 'Coco Coir Bulk' CREATE (b)-[:CREATED_USING]->(r);
MATCH (b:Bulk),  (r:Recipe) WHERE b.Id = 2 AND r.Name = 'CVG Bulk' CREATE (b)-[:CREATED_USING]->(r);

// Recipe > Ingredient
MATCH (i:Ingredient), (r:Recipe) WHERE i.Name IN ['Popcorn','Gypsum']      AND r.Id = 1 CREATE (r)-[:CREATED_USING]->(i);
MATCH (i:Ingredient), (r:Recipe) WHERE i.Name IN ['Food Colouring','Agar'] AND r.Id = 4 CREATE (r)-[:CREATED_USING]->(i);
MATCH (i:Ingredient), (r:Recipe) WHERE i.Name IN ['Coco-Coir'] AND r.Name = 'Coco Coir Bulk' CREATE (r)-[:CREATED_USING]->(i);
MATCH (i:Ingredient), (r:Recipe) WHERE i.Name IN ['Coco-Coir','Vermiculite','Gypsum'] AND r.Name = 'CVG Bulk' CREATE (r)-[:CREATED_USING]->(i);