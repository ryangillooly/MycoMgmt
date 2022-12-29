using MycoMgmt.API.Helpers;
using MycoMgmt.Domain.Models.Mushrooms;

namespace MycoMgmt.Helpers;

public static class CultureExtensions
{
    public static string Create(this Culture culture)
    {
        var additionalData = "";
        
        if (culture.Notes != null)
            additionalData += $",Notes: '{culture.Notes}'";
        
        var query = $@"CREATE 
                                (
                                    x:{culture.Tags[0]} {{ 
                                                         Name:       '{culture.Name}',
                                                         EntityType: '{culture.EntityType}',
                                                         Status:     '{culture.IsSuccessful()}'
                                                         {additionalData} 
                                                      }}
                                )
                            RETURN x";

        return query;
    }


    public static string? CreateVendorRelationship(this Culture culture)
    {
        return
            culture.Vendor is null
                ? null
                : $@"
                    MATCH 
                        (x:{culture.Tags[0]} {{ Name: '{culture.Name}'   }}),
                        (v:Vendor            {{ Name: '{culture.Vendor}' }})
                    MERGE
                        (x)-[r:PURCHASED_FROM]->(v)
                    RETURN 
                        r
                ";
    }
    
    public static string? UpdateVendorRelationship(this Culture culture)
    {
        return
            culture.Vendor is null
                ? null
                : $@"
                    MATCH 
                        (x:{culture.Tags[0]})
                    WHERE
                        elementId(x) = '{culture.ElementId}'
                    OPTIONAL MATCH
                        (x)-[r:PURCHASED_FROM]->(v)
                    DELETE 
                        r
                    WITH
                        x
                    MATCH
                        (v:Vendor  {{ Name: '{culture.Vendor}' }})
                    MERGE
                        (x)-[r:PURCHASED_FROM]->(v)
                    RETURN 
                        r
                ";
    }
    
    public static string GetAll(this Culture culture, int? skip, int? limit)
    {
        return
            $@"
                MATCH (x:{culture.EntityType})
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
                    count(elementId(x)) as entityCount
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
}