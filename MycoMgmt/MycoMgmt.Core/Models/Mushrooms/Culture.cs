﻿// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618

namespace MycoMgmt.Core.Models.Mushrooms
{
    public sealed class Culture : Mushroom
    {
        public Culture()
        {
        }
        public Culture
        (
            string     name,
            string     type,
            string     strain,
            string?    recipe,
            string?    notes,
            string?    location,
            string?    parent,
            string?    parentType,
            string?    children,
            string?    childType,
            string?    vendor,
            bool?      purchased,
            bool?      successful,
            bool?      finished,
            DateTime?  finishedOn,
            DateTime?  inoculatedOn,
            string?    inoculatedBy
        )
        {
            Name         = name;
            Type         = type;
            Notes        = notes;
            Strain       = strain;
            Location     = location;
            Parent       = parent;
            ParentType   = parentType;
            Children     = children;
            ChildType    = childType;
            Successful   = successful;
            Finished     = finished;
            FinishedOn   = finishedOn;
            InoculatedOn = inoculatedOn; 
            InoculatedBy = inoculatedBy;
            Recipe       = recipe;
            Status       = IsSuccessful();
            Purchased    = purchased;
            Vendor       = vendor;
            Tags         = new List<string> { IsSuccessful() };
        }
        
        public override string CreateNode()
        {
            var additionalData = "";
        
            if (Notes != null)
                additionalData += $",Notes: '{Notes}'";
            
            // var status = IsSuccessful();
            var query = $@"CREATE 
                                (
                                    x
                                        :`{EntityType}`
                                        :`{Type}`
                                        :`{Status}`
                                    {{ 
                                        Name:       '{Name}',
                                        Id:         '{Id}',
                                        EntityType: '{EntityType}',
                                        Type:       '{Type}',
                                        Status:     '{Status}'
                                        {additionalData} 
                                     }}
                                )
                            RETURN x";

            return query;
        }
        public override List<string> CreateQueryList()
        {
            var queryList = new List<string>
            {
                CreateNode(),
                CreateInoculatedRelationship(),
                CreateInoculatedOnRelationship(),
                CreateFinishedOnRelationship(),
                CreateStrainRelationship(),
                CreateLocationRelationship(),
                CreateCreatedRelationship(),
                CreateCreatedOnRelationship(),
                CreateParentRelationship(),
                CreateRecipeRelationship(),
                CreateChildRelationship(),
                CreateNodeLabels(),
                CreateVendorRelationship()
            };
            
            queryList.RemoveAll(item => item is null);
            return queryList;
        }
        public override List<string> UpdateQueryList()
        {
            var queryList = new List<string>
            {
                UpdateName(),
                UpdateNotes(),
                UpdateType(),
                UpdateInoculatedRelationship(),
                UpdateInoculatedOnRelationship(),
                UpdateFinishedOnRelationship(),
                UpdateRecipeRelationship(),
                UpdateStrainRelationship(),
                UpdateLocationRelationship(),
                UpdateParentRelationship(),
                UpdateChildRelationship(),
                UpdateVendorRelationship(),
                UpdateModifiedOnRelationship(),
                UpdateModifiedRelationship(),
                UpdateStatus(),
                UpdateStatusLabel(),
            };
            
            queryList.RemoveAll(item => item is null);
            return queryList;
        }
        
        /*
        public override string GetByIdQuery()
        {
            return
                $@"
                    MATCH (x:{EntityType})
                    WHERE x.Id = '{Id}'
                    OPTIONAL MATCH (x)<-[:HAS_PARENT]-(child)
                    WITH 
                        x,
                        collect(child.Name) as children
                        OPTIONAL MATCH (x)-[:INOCULATED_ON]->(iDay:Day)<-[:HAS_DAY]-(iMonth:Month)-[:HAS_MONTH]-(iYear:Year)
                        OPTIONAL MATCH (x)-[:CREATED_ON]   ->(cDay:Day)<-[:HAS_DAY]-(cMonth:Month)-[:HAS_MONTH]-(cYear:Year)
                        OPTIONAL MATCH (x)-[:MODIFIED_ON]  ->(mDay:Day)<-[:HAS_DAY]-(mMonth:Month)-[:HAS_MONTH]-(mYear:Year)
                        OPTIONAL MATCH (x)-[:FINISHED_ON]  ->(fDay:Day)<-[:HAS_DAY]-(fMonth:Month)-[:HAS_MONTH]-(fYear:Year)
                        OPTIONAL MATCH (cUser:User)-[:CREATED]   ->(x)
                        OPTIONAL MATCH (mUser:User)-[:MODIFIED]  ->(x)
                        OPTIONAL MATCH (iUser:User)-[:INOCULATED]->(x)
                        OPTIONAL MATCH (x)-[:HAS_STRAIN]->(strain:Strain)
                        OPTIONAL MATCH (x)-[:STORED_IN]->(location:Location)
                        OPTIONAL MATCH (x)-[:HAS_PARENT]->(parent)
                        OPTIONAL MATCH (x)-[:CREATED_USING]->(recipe:Recipe)
                        OPTIONAL MATCH (x)-[:PURCHASED_FROM]->(vendor:Vendor)
                    WITH
                        x, 
                        reduce(x = '', i IN children | x + i + ',') as childrenString,
                        datetime({{year: iYear.year, month: iMonth.month, day: iDay.day}}) as inoculatedDate,
                        datetime({{year: cYear.year, month: cMonth.month, day: cDay.day}}) as createdDate,
                        datetime({{year: mYear.year, month: mMonth.month, day: mDay.day}}) as modifiedDate,
                        datetime({{year: fYear.year, month: fMonth.month, day: fDay.day}}) as finishedDate,
                        properties(cUser).Name as createdBy,
                        properties(mUser).Name as modifiedBy,
                        properties(iUser) as inoculatedBy,
                        properties(strain).Name as strain,
                        properties(location).Name as location,
                        parent,
                        vendor,
                        recipe
                    WITH    
                        x,
                        left(childrenString, size(childrenString)-1) as children,
                        inoculatedDate,
                        createdDate,
                        modifiedDate,
                        finishedDate,
                        createdBy,
                        modifiedBy,
                        inoculatedBy,
                        strain,
                        location,
                        parent,
                        vendor,
                        recipe
                    RETURN
                        apoc.map.mergeList
                        ([
                            {{Id:           x.Id}},
                            {{EntityType:   properties(x).EntityType}},
                            {{Type:         properties(x).Type}},
                            {{Name:         properties(x).Name}},
                            {{Notes:        properties(x).Notes}},
                            {{Strain:       strain}},
                            {{Status:       properties(x).Status}},
                            {{Location:     location}},
                            {{Recipe:       properties(recipe).Name}},
                            {{Vendor:       properties(vendor).Name}},
                            {{Parent:       properties(parent).Name}},
                            {{ParentType:   properties(parent).EntityType}},
                            {{Children:     children}},
                            {{InoculatedOn: apoc.date.toISO8601(inoculatedDate.epochMillis, 'ms')}},
                            {{InoculatedBy: inoculatedBy.Name}},
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
                ";
        }
        
        public override string GetAllQuery(int skip = 0, int limit = 20)
        {
            return
                $@"
                    MATCH (x:{EntityType})
                        OPTIONAL MATCH (x)<-[:HAS_PARENT]-(child)
                    WITH 
                        x,
                        collect(child.Name) as children
                        OPTIONAL MATCH (x)-[:INOCULATED_ON]->(iDay:Day)<-[:HAS_DAY]-(iMonth:Month)-[:HAS_MONTH]-(iYear:Year)
                        OPTIONAL MATCH (x)-[:CREATED_ON]   ->(cDay:Day)<-[:HAS_DAY]-(cMonth:Month)-[:HAS_MONTH]-(cYear:Year)
                        OPTIONAL MATCH (x)-[:MODIFIED_ON]  ->(mDay:Day)<-[:HAS_DAY]-(mMonth:Month)-[:HAS_MONTH]-(mYear:Year)
                        OPTIONAL MATCH (x)-[:FINISHED_ON]  ->(fDay:Day)<-[:HAS_DAY]-(fMonth:Month)-[:HAS_MONTH]-(fYear:Year)
                        OPTIONAL MATCH (cUser:User)-[:CREATED]   ->(x)
                        OPTIONAL MATCH (mUser:User)-[:MODIFIED]  ->(x)
                        OPTIONAL MATCH (iUser:User)-[:INOCULATED]->(x)
                        OPTIONAL MATCH (x)-[:HAS_STRAIN]->(strain:Strain)
                        OPTIONAL MATCH (x)-[:STORED_IN]->(location:Location)
                        OPTIONAL MATCH (x)-[:HAS_PARENT]->(parent)
                        OPTIONAL MATCH (x)-[:CREATED_USING]->(recipe:Recipe)
                    WITH
                        x, 
                        reduce(x = '', i IN children | x + i + ',') as childrenString,
                        datetime({{year: iYear.year, month: iMonth.month, day: iDay.day}}) as inoculatedDate,
                        datetime({{year: cYear.year, month: cMonth.month, day: cDay.day}}) as createdDate,
                        datetime({{year: mYear.year, month: mMonth.month, day: mDay.day}}) as modifiedDate,
                        datetime({{year: fYear.year, month: fMonth.month, day: fDay.day}}) as finishedDate,
                        properties(cUser).Name as createdBy,
                        properties(mUser).Name as modifiedBy,
                        properties(iUser) as inoculatedBy,
                        properties(strain).Name as strain,
                        properties(location).Name as location,
                        parent,
                        count(x.Id) as entityCount
                    WITH    
                        x,
                        left(childrenString, size(childrenString)-1) as children,
                        inoculatedDate,
                        createdDate,
                        modifiedDate,
                        finishedDate,
                        createdBy,
                        modifiedBy,
                        inoculatedBy,
                        strain,
                        location,
                        parent,
                        sum(entityCount) as entities
                    RETURN
                        entities, 
                        apoc.map.mergeList
                        ([
                            {{Id:           x.Id}},
                            {{EntityType:   properties(x).EntityType}},
                            {{Type:         properties(x).Type}},
                            {{Name:         properties(x).Name}},
                            {{Notes:        properties(x).Notes}},
                            {{Strain:       strain}},
                            {{Status:       properties(x).Status}},
                            {{Location:     location}},
                            {{Parent:       properties(parent).Name}},
                            {{ParentType:   properties(parent).EntityType}},
                            {{Children:     children}},
                            {{InoculatedOn: apoc.date.toISO8601(inoculatedDate.epochMillis, 'ms')}},
                            {{InoculatedBy: inoculatedBy.Name}},
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
        }
        */
    }
}