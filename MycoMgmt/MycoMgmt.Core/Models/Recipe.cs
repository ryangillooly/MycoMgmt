namespace MycoMgmt.Domain.Models
{
    public class Recipe : ModelBase
    {
        public string? Description { get; set; }
        public List<string>? Steps { get; set; }
        public List<string>? Ingredients { get; set; }
        
        // Create
        public override string CreateNode()
        {
            var additionalData = "";
        
            if (Notes != null)
                additionalData += $",Notes: '{Notes}'";

            if (Description != null)
                additionalData += $",Description: '{Description}'";
        
            if (Steps != null)
                additionalData += $",Steps: '{StepsToNumberedStringList()}'";
        
            var query = $@"
                            CREATE 
                            (
                                x:{EntityType} {{ 
                                                     Name: '{Name}',
                                                     Type: '{Type}'
                                                     {additionalData} 
                                                  }}
                            )
                            RETURN x
                            ";

            return query;
        }
        private string? CreateIngredientRelationship()
        {
            return
                Ingredients is null
                    ? null
                    : $@"
                          MATCH 
                              (recipe:{EntityType} {{ Name: '{Name}' }}), 
                              (i:Ingredient)
                          WHERE
                              i.Name IN ['{string.Join("','", Ingredients)}']
                          MERGE
                              (recipe)-[r:CREATED_USING]->(i)
                          RETURN r
                      ";
        }
        public override List<string> CreateQueryList()
        {
            var queryList = new List<string>
            {
                CreateNode(),
                CreateIngredientRelationship(),
                CreateCreatedRelationship(),
                CreateCreatedOnRelationship()
            };

            queryList.RemoveAll(item => item is null);
            return queryList;
        }
        
        // Update
        public override List<string> UpdateQueryList()
        {
            var queryList = new List<string>
            {
                UpdateName(),
                UpdateType(),
                UpdateSteps(),
                UpdateDescription(),
                UpdateNotes(),
                UpdateIngredientRelationship(),
                UpdateModifiedOnRelationship(),
                UpdateModifiedRelationship()
            };

            queryList.RemoveAll(item => item is null);
            return queryList;
        }
        private string? UpdateIngredientRelationship()
        {
            return
                Ingredients is null
                    ? null
                    : $@" 
                        MATCH 
                            (recipe:{EntityType})
                        WHERE
                            elementId(recipe) = '{ElementId}'
                        OPTIONAL MATCH
                            (recipe)-[r:CREATED_USING]->(i)
                        DELETE
                            r
                        WITH
                            recipe
                        MATCH
                            (i:Ingredient)
                        WHERE
                            i.Name IN ['{string.Join("','", Ingredients)}']
                        MERGE 
                            (recipe)-[r:CREATED_USING]->(i)
                        RETURN
                            r  
                      ";
        }
        private string? UpdateSteps()
        {
            return
                Steps is null
                    ? null
                    : $@"
                        MATCH 
                            (x:{EntityType}) 
                        WHERE 
                            elementId(x) = '{ElementId}' 
                        SET 
                            x.Steps = '{StepsToNumberedStringList()}' 
                        RETURN 
                            x
                      ";
        }
        private string? UpdateDescription()
        {
            return
                Description is null
                    ? null
                    : $@"
                        MATCH 
                            (x:{EntityType}) 
                        WHERE 
                            elementId(x) = '{ElementId}' 
                        SET 
                            x.Description = '{Description}' 
                        RETURN 
                            x
                      ";
        }

        // Read
        public override string GetAllQuery(int skip, int limit)
        {
             var query =
                $@"
                    MATCH (x:{EntityType})
                    OPTIONAL MATCH (x)<-[:HAS_PARENT]-(child)
                    WITH 
                        x,
                        collect(child.Name) as children
                    OPTIONAL MATCH (x)-[:CREATED_ON]   ->(cDay:Day)<-[:HAS_DAY]-(cMonth:Month)-[:HAS_MONTH]-(cYear:Year)
                    OPTIONAL MATCH (x)-[:MODIFIED_ON]  ->(mDay:Day)<-[:HAS_DAY]-(mMonth:Month)-[:HAS_MONTH]-(mYear:Year)
                    OPTIONAL MATCH (x)-[:FINISHED_ON]  ->(fDay:Day)<-[:HAS_DAY]-(fMonth:Month)-[:HAS_MONTH]-(fYear:Year)
                    OPTIONAL MATCH (cUser:User)-[:CREATED]   ->(x)
                    OPTIONAL MATCH (mUser:User)-[:MODIFIED]  ->(x)
                    OPTIONAL MATCH (x)-[:HAS_STRAIN]->(strain:Strain)
                    OPTIONAL MATCH (x)-[:STORED_IN]->(location:Location)
                    OPTIONAL MATCH (x)-[:HAS_PARENT]->(parent)
                    OPTIONAL MATCH (x)-[:CREATED_USING]->(recipe:Recipe)
                    WITH
                        x, 
                        reduce(x = '', i IN children | x + i + ',') as childrenString,
                        datetime({{year: cYear.year, month: cMonth.month, day: cDay.day}}) as createdDate,
                        datetime({{year: mYear.year, month: mMonth.month, day: mDay.day}}) as modifiedDate,
                        datetime({{year: fYear.year, month: fMonth.month, day: fDay.day}}) as finishedDate,
                        properties(cUser).Name as createdBy,
                        properties(mUser).Name as modifiedBy,
                        properties(strain).Name as strain,
                        properties(location).Name as location,
                        parent
                    WITH    
                        x,
                        left(childrenString, size(childrenString)-1) as children,
                        createdDate,
                        modifiedDate,
                        finishedDate,
                        createdBy,
                        modifiedBy,
                        strain,
                        location,
                        parent
                    RETURN 
                        apoc.map.mergeList
                        ([
                            {{ElementId:    elementId(x)}},
                            {{Name:         properties(x).Name}},
                            {{EntityType:   properties(x).EntityType}},
                            {{Notes:        properties(x).Notes}},
                            {{Strain:       strain}},
                            {{Status:       properties(x).Status}},
                            {{Location:     location}},
                            {{Parent:       properties(parent).Name}},
                            {{ParentType:   properties(parent).EntityType}},
                            {{Children:     children}},
                            {{CreatedOn:    apoc.date.toISO8601(createdDate.epochMillis, 'ms')}},
                            {{CreatedBy:    createdBy}},
                            {{ModifiedOn:   apoc.date.toISO8601(modifiedDate.epochMillis, 'ms')}},        
                            {{ModifiedBy:   modifiedBy}},
                            {{FinishedOn:   apoc.date.toISO8601(finishedDate.epochMillis, 'ms')}}
                        ])
                        as result
                    ORDER BY
                        finishedDate.day   DESC,
                        finishedDate.month DESC,
                        finishedDate.year  DESC,
                        properties(x).Name ASC
                    SKIP 
                        {skip}
                    LIMIT
                        {limit}
                ";

             return query;
        }
        private string StepsToNumberedStringList() => Steps is null ? null : string.Join(", ", Steps.Select((item, index) => $"{index + 1}.{item}"));
    }
}