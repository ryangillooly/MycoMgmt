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
                OPTIONAL MATCH (x)<-[:HAS_PARENT]-(child)
                OPTIONAL MATCH (x)-[:CREATED_USING]->(recipe:Recipe)
                WITH 
                    x, 
                    datetime({{year: iYear.year, month: iMonth.month, day: iDay.day}}) as inoculatedDate,
                    datetime({{year: cYear.year, month: cMonth.month, day: cDay.day}}) as createdDate,
                    datetime({{year: mYear.year, month: mMonth.month, day: mDay.day}}) as modifiedDate,
                    datetime({{year: fYear.year, month: fMonth.month, day: fDay.day}}) as finishedDate,
                    properties(cUser) as createdBy,
                    properties(mUser) as modifiedBy,
                    properties(iUser) as inoculatedBy,
                    properties(strain) as strain,
                    properties(location) as location,
                    recipe,
                    parent, 
                    child
                RETURN 
                    apoc.map.mergeList
                    ([
                        {{ElementId:    elementId(x)}},
                        {{Name:         properties(x).Name}},
                        {{Type:         labels(x)[1]}},
                        {{Notes:        properties(x).Notes}},
                        {{Strain:       strain.Name}},
                        {{Status:       properties(x).Status}},
                        {{Recipe:       recipe.Name}},
                        {{Location:     location.Name}},
                        {{Parent:       properties(parent).Name}},
                        {{ParentType:   labels(parent)[0]}},
                        {{Children:        properties(child).Name}},
                        {{ChildType:    labels(child)[0]}},
                        {{InoculatedOn: apoc.date.toISO8601(inoculatedDate.epochMillis, 'ms')}},
                        {{InoculatedBy: inoculatedBy.Name}},
                        {{CreatedOn:    apoc.date.toISO8601(createdDate.epochMillis, 'ms')}},
                        {{CreatedBy:    createdBy.Name}},
                        {{ModifiedOn:   apoc.date.toISO8601(modifiedDate.epochMillis, 'ms')}},        
                        {{ModifiedBy:   modifiedBy.Name}},
                        {{FinishedOn:   apoc.date.toISO8601(finishedDate.epochMillis, 'ms')}}
                    ])
                    as result
                ORDER BY
                    inoculatedDate.day   DESC,
                    inoculatedDate.month DESC,
                    inoculatedDate.year  DESC
                SKIP 
                    {skip}
                LIMIT
                    {limit}
            ";
    }
}