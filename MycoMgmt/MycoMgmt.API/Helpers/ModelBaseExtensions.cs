using MycoMgmt.Domain.Models;

namespace MycoMgmt.API.Helpers;

public static class ModelBaseExtensions
{
    public static string? UpdateModifiedOnRelationship(this ModelBase mushroom)
    {
        DateTime.TryParse(mushroom.ModifiedOn.ToString(), out var parsedDateTime);

        var query = $@"
                            MATCH 
                                (x:{mushroom.Tags[0]})
                            WHERE
                                elementId(x) = '{mushroom.ElementId}'
                            OPTIONAL MATCH
                                (x)-[r:MODIFIED_ON]->(d)
                            DELETE 
                                r
                            WITH
                                x
                            MATCH
                                (d:Day {{ day: {parsedDateTime.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {parsedDateTime.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {parsedDateTime.Year} }})
                            MERGE
                                (x)-[r:MODIFIED_ON]->(d)
                            RETURN 
                                r
                        ";

        return query;
    }

    public static string? UpdateModifiedRelationship(this ModelBase mushroom)
    {
        return
            $@"
                MATCH 
                    (x:{mushroom.Tags[0]})
                WHERE
                    elementId(x) = '{mushroom.ElementId}'
                OPTIONAL MATCH
                    (u)-[r:MODIFIED]->(x)
                DELETE
                    r
                WITH
                    x
                MATCH
                    (u:User {{ Name: '{mushroom.ModifiedBy}'}} )
                MERGE 
                    (u)-[r:MODIFIED]->(x)
                RETURN
                    r  
            ";
    }

    public static string? CreateCreatedRelationship(this ModelBase node)
    {
        return
            $@"
                MATCH 
                    (x:{node.Tags[0]} {{ Name: '{node.Name}'      }}),
                    (u:User               {{ Name: '{node.CreatedBy}' }})
                CREATE
                    (u)-[r:CREATED]->(x)
                RETURN r
            ";
    }

    public static string? CreateCreatedOnRelationship(this ModelBase mushroom)
    {
        return
            $@"
                MATCH 
                    (x:{mushroom.Tags[0]} {{ Name: '{mushroom.Name}' }}), 
                    (d:Day                {{ day:   {mushroom.CreatedOn.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {mushroom.CreatedOn.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {mushroom.CreatedOn.Year} }})
                CREATE
                    (x)-[r:CREATED_ON]->(d)
                RETURN r
            ";
    }
    
    public static string? Delete(this ModelBase node)
    {
        return
            node.ElementId is null
                ? null
                : $@"
                    MATCH 
                        (x:{node.Tags[0]}) 
                    WHERE 
                        elementId(x) = '{ node.ElementId }' 
                    DETACH DELETE 
                        x
                    RETURN 
                        x
                ";
    }
    
    public static string? UpdateName(this ModelBase node)
    {
        return
            node.Name is null
                ? null
                : $@"
                    MATCH 
                        (x:{node.Tags[0]}) 
                    WHERE 
                        elementId(x) = '{node.ElementId}' 
                    SET 
                        x.Name = '{node.Name}' 
                    RETURN 
                        x
                  ";
    }
    
    public static string? UpdateNotes(this ModelBase nodes)
    {
        return
            nodes.Notes is null
                ? null
                : $@"
                    MATCH 
                        (x:{nodes.Tags[0]}) 
                    WHERE 
                        elementId(x) = '{nodes.ElementId}' 
                    SET 
                        x.Notes = '{nodes.Notes}' 
                    RETURN s 
                  ";
    }
    
    public static string? UpdateType(this ModelBase node)
    {
        return
            node.Type is null
                ? null
                : $@"
                    MATCH 
                        (x:{node.Tags[0]}) 
                    WHERE 
                        elementId(x) = '{node.ElementId}' 
                    SET 
                        x.Type = '{node.Type}' 
                    RETURN s 
                  ";
    }
    
    public static string SearchByNameQuery(this ModelBase node)
    {
        return
            $@"
                MATCH 
                    (x:{node.Tags[0]}) 
                WHERE 
                    toUpper(x.Name) CONTAINS toUpper('{node.Name}') 
                RETURN 
                    x 
                ORDER BY 
                    x.Name ASC
                LIMIT 
                    100
            ";
    }

    public static string GetByNameQuery(this ModelBase node)
    {
        return
            $@"
                MATCH 
                    (x:{node.Tags[0]}) 
                WHERE 
                    toUpper(x.Name) = toUpper('{node.Name}') 
                RETURN 
                    x
            ";
    }

    public static string GetByIdQuery(this ModelBase node)
    {
        return
            $@"
                MATCH 
                    (x:{node.Tags[0]}) 
                WHERE 
                    elementId(x) = '{node.ElementId}'
                RETURN 
                    x
            ";
    }

    public static string GetAll(this ModelBase node, int? skip, int? limit)
    {
        /*
        return
            $@"
                MATCH 
                    (x:{node.Tags[0]})
                OPTIONAL MATCH 
                    (x)-[r:INOCULATED_ON]->(d:Day)<-[rd:HAS_DAY]-(m:Month)-[:HAS_MONTH]-(y:Year)
                WITH 
                    x, y, m, d
                RETURN 
                    x as Culture, 
                    datetime({{year: y.year, month: m.month, day: d.day}}) as InoculationDate
                ORDER BY
                    d.day   DESC,
                    m.month DESC,
                    y.year  DESC
                SKIP 
                    {skip}
                LIMIT
                    {limit}
            ";
        */

        var a =
            $@"
                MATCH (x:{node.Tags[0]})
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
                        CASE
                            WHEN labels(x)[2] = 'InProgress' THEN {{Finished: false}}
                            WHEN labels(x)[2] = 'Failed'     THEN {{Finished: true}}
                            WHEN labels(x)[2] = 'Successful' THEN {{Finished: true}}
                        END,
                        CASE
                            WHEN labels(x)[2] = 'Successful' THEN {{Successful: true}}
                            WHEN labels(x)[2] = 'Failed'     THEN {{Successful: false}}
                            WHEN labels(x)[2] = 'InProgress' THEN {{Successful: false}}
                        END,
                        {{Recipe:       recipe.Name}},
                        {{Location:     location.Name}},
                        {{Parent:       properties(parent).Name}},
                        {{ParentType:   labels(parent)[0]}},
                        {{Child:        properties(child).Name}},
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

        return a;
    }
}