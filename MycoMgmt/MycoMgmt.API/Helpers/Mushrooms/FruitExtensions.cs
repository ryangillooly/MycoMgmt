using MycoMgmt.API.Helpers;
using MycoMgmt.Domain.Models.Mushrooms;

namespace MycoMgmt.Helpers;

public static class FruitExtensions
{
    public static string Create(this Fruit fruit)
    {
        var additionalData = "";
        
        if (fruit.WetWeight != null)
            additionalData += $",WetWeight: {fruit.WetWeight}";
        
        if (fruit.DryWeight != null)
            additionalData += $",DryWeight: {fruit.DryWeight}";
        
        if (fruit.Notes != null)
            additionalData += $",Notes: '{fruit.Notes}'";
        
        var query = $@"CREATE 
                                (
                                    x:{fruit.Tags[0]} {{ 
                                                         Name: '{fruit.Name}',
                                                         EntityType: '{fruit.EntityType}',
                                                         Status: '{fruit.IsSuccessful()}'
                                                         {additionalData} 
                                                      }}
                                )
                            RETURN x";

        return query;
    }
    
    public static string GetAll(this Fruit fruit, int? skip, int? limit)
    {
         var query =
            $@"
                MATCH (x:{fruit.EntityType})
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
}