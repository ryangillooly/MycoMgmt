namespace MycoMgmt.Domain.Models.Mushrooms;

public static class MushroomExtensions
{
    public static string ToNodeLabelQuery(this Mushroom mushroom) => 
    $@"
        MATCH (x:{mushroom.Tags[0]} {{ Name: '{mushroom.Name}' }})
        SET x:{string.Join(":", mushroom.Tags)}
        RETURN x
    ";

    public static string ToParentQuery(this Mushroom mushroom) =>
    $@"
        MATCH 
            (c:{mushroom.Tags[0]} {{ Name: '{mushroom.Name}' }}), 
            (p {{ Name: '{mushroom.Parent}' }})
        MERGE
            (c)-[r:HAS_PARENT]->(p)
        RETURN r
    ";

    public static string ToCreatedQuery(this Mushroom mushroom) =>
    $@"
        MATCH 
            (x:{mushroom.Tags[0]} {{ Name: '{mushroom.Name}' }}),
            (u:User {{ Name: '{mushroom.CreatedBy}' }})
        MERGE
            (u)-[r:CREATED]->(x)
        RETURN r
    ";

    public static string ToCreatedOnQuery(this Mushroom mushroom) =>
    $@"
        MATCH 
            (x:{mushroom.Tags[0]} {{ Name: '{mushroom.Name}' }}), 
            (d:Day {{ day: {mushroom.CreatedOn.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {mushroom.CreatedOn.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {mushroom.CreatedOn.Year} }})
        MERGE
            (x)-[r:CREATED_ON]->(d)
        RETURN r
    ";
    
    public static string IsSuccessful(this Mushroom mushroom)
    {
        if ((bool)!mushroom.Finished) 
            return "InProgress";

        return mushroom.Successful switch
        {
            true => "Successful",
            false => "Failed"
        };
    }
}