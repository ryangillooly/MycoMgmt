using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace MycoMgmt.Core.Models.Mushrooms
{
    public class Fruit : Mushroom
    {
        public Fruit()
        {
        
        }

        public Fruit
        (
            string    name,
            string    strain,
            decimal?  wetWeight,
            decimal?  dryWeight,
            string?   notes,
            string?   location,
            string?   parent,
            string?   parentType,
            string?   children,
            string?   childType,
            string?   vendor,
            bool?     purchased,
            bool?     successful,
            bool?     finished
        )
        {
            Name        = name;
            WetWeight   = wetWeight;
            DryWeight   = dryWeight;
            Notes       = notes;
            Strain      = strain;
            Location    = location;
            Parent      = parent;
            ParentType  = parentType;
            Children    = children;
            ChildType   = childType;
            Successful  = successful;
            Finished    = finished;
            Purchased   = purchased;
            Vendor      = vendor;
        }
        
        public DateTime? HarvestedOn { get; set; }
        public string? HarvestedBy { get; set; }
        public decimal? WetWeight { get; set; }
        public decimal? DryWeight { get; set; }
        
        // Create
        public override string CreateNode()
        {
            var additionalData = "";
            
            if (WetWeight != null)
                additionalData += $",WetWeight: {WetWeight}";
            
            if (DryWeight != null)
                additionalData += $",DryWeight: {DryWeight}";
            
            if (Notes != null)
                additionalData += $",Notes: '{Notes}'";
            
            var query = $@"CREATE 
                                    (
                                        x:{EntityType} {{ 
                                                             Name:       '{Name}',
                                                             Id:       '{Id}',
                                                             EntityType: '{EntityType}',
                                                             Status:     '{IsSuccessful()}'
                                                             {additionalData} 
                                                          }}
                                    )
                                RETURN x";

            return query;
        }
        
        public virtual string? CreateHarvestedRelationship()
        {
            return
                HarvestedBy is null
                    ? null
                    : $@"
                          MATCH 
                              (x:{EntityType} {{ Name: '{Name}' }}),
                              (u:User {{ Name: '{HarvestedBy}' }})
                          CREATE
                              (u)-[r:HARVESTED]->(x)
                          RETURN r
                      ";
        }
        
        public virtual string? CreateHarvestedOnRelationship()
        {
            return
                HarvestedOn is null
                ? null
                : $@"
                    MATCH 
                        (x:{EntityType} {{ Name: '{Name}' }}), 
                        (d:Day {{ day: {HarvestedOn.Value.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {HarvestedOn.Value.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {HarvestedOn.Value.Year} }})
                    CREATE
                        (x)-[r:HARVESTED_ON]->(d)
                    RETURN r
                 ";
        }
        
        public virtual string? UpdateHarvestedRelationship()
        {
            return $@"
                        MATCH 
                            (x:{EntityType})
                        WHERE
                            x.Id = '{Id}'
                        OPTIONAL MATCH
                            (u:User)-[r:HARVESTED]->(x)
                        DELETE 
                            r
                        WITH
                            x
                        MATCH
                            (u:User {{ Name: '{InoculatedBy}' }})
                        CREATE
                            (u)-[r:HARVESTED]->(x)
                        RETURN 
                            r
                    ";
        }
        
        public virtual string? UpdateHarvestedOnRelationship()
        {
            return 
                HarvestedOn is null
                    ? null
                    : $@"
                        MATCH 
                            (x:{EntityType})
                        WHERE
                            x.Id = '{Id}'
                        OPTIONAL MATCH
                            (x)-[r:HARVESTED_ON]->(d)
                        DELETE 
                            r
                        WITH
                            x
                        MATCH
                            (d:Day {{ day: {HarvestedOn.Value.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {HarvestedOn.Value.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {HarvestedOn.Value.Year} }})
                        CREATE
                            (x)-[r:HARVESTED_ON]->(d)
                        RETURN 
                            r
                    ";
        }
        
        public override List<string> CreateQueryList()
        {
            var queryList = new List<string>
            {
                CreateNode(),
                CreateInoculatedRelationship(),
                CreateInoculatedOnRelationship(),
                CreateHarvestedRelationship(),
                CreateHarvestedOnRelationship(),
                CreateFinishedOnRelationship(),
                CreateStrainRelationship(),
                CreateLocationRelationship(),
                CreateCreatedRelationship(),
                CreateCreatedOnRelationship(),
                CreateParentRelationship(),
                CreateChildRelationship(),
                CreateVendorRelationship(),
                CreateNodeLabels()
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
                UpdateInoculatedRelationship(),
                UpdateInoculatedOnRelationship(),
                UpdateHarvestedRelationship(),
                UpdateHarvestedOnRelationship(),
                UpdateFinishedOnRelationship(),
                UpdateRecipeRelationship(),
                UpdateLocationRelationship(),
                UpdateParentRelationship(),
                UpdateChildRelationship(),
                UpdateModifiedOnRelationship(),
                UpdateModifiedRelationship(),
                UpdateVendorRelationship(),
                UpdateStatus(),
                UpdateStatusLabel()
            };

            queryList.RemoveAll(item => item is null);
            return queryList;
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
                            {{Id:    x.Id}},
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
    }
}