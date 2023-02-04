using MycoMgmt.Core.Models;

namespace MycoMgmt.Core.Extensions;

public static class ModelBaseExtensions
{
    public static string ToGetQuery(this ModelBase input, Guid? id = null, int? skip = null, int? limit = null)
    {
        var filterString = "";
        var skipString   = "";
        var limitString  = "";
        
        if (id    is not null) filterString = $"WHERE x.Id = \"{id}\"";
        if (skip  is not null) skipString   = $"SKIP {skip}";
        if (limit is not null) limitString  = $"LIMIT {limit}";  

        return 
            $@"
                MATCH (x:{input.EntityType})
                    {filterString}
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
                    recipe,
                    vendor,
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
                    recipe,
                    vendor,
                    sum(entityCount) as entities
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
                {skipString}
                {limitString}
            ";
    }
}