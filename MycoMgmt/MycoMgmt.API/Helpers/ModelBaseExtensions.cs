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
        
        
    }
}