using MycoMgmt.Domain.Models;

namespace MycoMgmt.API.Helpers;

public static class ModelBaseExtensions
{
    public static string? UpdateModifiedOnRelationship(this ModelBase mushroom, string elementId)
    {
        DateTime.TryParse(mushroom.ModifiedOn.ToString(), out var parsedDateTime);

        var query = $@"
                            MATCH 
                                (x:{mushroom.Tags[0]})
                            WHERE
                                elementId(x) = '{elementId}'
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

    public static string? UpdateModifiedRelationship(this ModelBase mushroom, string elementId)
    {
        return
            $@"
                MATCH 
                    (x:{mushroom.Tags[0]})
                WHERE
                    elementId(x) = '{elementId}'
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

    public static string CreateCreatedRelationship(this ModelBase mushroom)
    {
        return
            $@"
                MATCH 
                    (x:{mushroom.Tags[0]} {{ Name: '{mushroom.Name}'      }}),
                    (u:User               {{ Name: '{mushroom.CreatedBy}' }})
                MERGE
                    (u)-[r:CREATED]->(x)
                RETURN r
            ";
    }

    public static string CreateCreatedOnRelationship(this ModelBase mushroom)
    {
        return
            $@"
                MATCH 
                    (x:{mushroom.Tags[0]} {{ Name: '{mushroom.Name}' }}), 
                    (d:Day                {{ day:   {mushroom.CreatedOn.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {mushroom.CreatedOn.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {mushroom.CreatedOn.Year} }})
                MERGE
                    (x)-[r:CREATED_ON]->(d)
                RETURN r
            ";
    }
}