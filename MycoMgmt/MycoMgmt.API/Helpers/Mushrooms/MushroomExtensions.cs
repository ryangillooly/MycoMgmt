namespace MycoMgmt.Domain.Models.Mushrooms;

public static class MushroomExtensions
{
    public static string? UpdateModifiedOnRelationship(this Mushroom mushroom, string elementId)
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

    public static string? UpdateModifiedRelationship(this Mushroom mushroom, string elementId) =>
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

    public static string? UpdateParentRelationship(this Mushroom mushroom, string elementId)
    {
        return
            mushroom.Parent is null
                ? null
                : $@"
                    MATCH 
                        (c:{mushroom.Tags[0]})
                    WHERE
                        elementId(c) = '{elementId}'
                    OPTIONAL MATCH
                        (c)-[r:HAS_PARENT]->(p)
                    DELETE
                        r
                    WITH
                        c
                    MATCH 
                        (p:{mushroom.ParentType} {{Name: '{mushroom.Parent}' }})
                    MERGE 
                        (c)-[r:HAS_PARENT]->(p) 
                    RETURN 
                        r
                ";
    }

public static string? UpdateChildRelationship(this Mushroom mushroom, string elementId)
    {
        return
            mushroom.Child is null
                ? null
                : $@"
                    MATCH 
                        (p:{mushroom.Tags[0]})
                    WHERE
                        elementId(p) = '{elementId}'
                    OPTIONAL MATCH
                        (c)-[r:HAS_PARENT]->(p)
                    DELETE
                        r
                    WITH
                        p
                    MATCH 
                        (c:{mushroom.ChildType} {{Name: '{mushroom.Child}' }})
                    MERGE 
                        (c)-[r:HAS_PARENT]->(p) 
                    RETURN 
                        r
                ";
    }

    public static string? UpdateStrainRelationship(this Mushroom mushroom, string elementId)
    {
        return
            mushroom.Strain is null
                ? null
                : $@"
                    MATCH 
                        (x:{mushroom.Tags[0]})
                    WHERE
                        elementId(x) = '{elementId}'
                    OPTIONAL MATCH
                        (x)-[r:HAS_STRAIN]->(:Strain)
                    DELETE 
                        r
                    WITH
                        x
                    MATCH
                        (s:Strain {{ Name: '{mushroom.Strain}' }})
                    MERGE
                        (x)-[r:HAS_STRAIN]->(s)
                    RETURN 
                        r
                ";
    }

    public static string? UpdateLocationRelationship(this Mushroom mushroom, string elementId)
    {
        return
            mushroom.Location is null
                ? null
                : $@"
                    MATCH 
                        (x:{mushroom.Tags[0]})
                    WHERE
                        elementId(x) = '{elementId}'
                    OPTIONAL MATCH
                        (x)-[r:STORED_IN]->(:Location)
                    DELETE 
                        r
                    WITH
                        x
                    MATCH
                        (l:Location {{ Name: '{mushroom.Location}' }})
                    MERGE
                        (x)-[r:STORED_IN]->(l) 
                    RETURN 
                        r
                ";
    }

    public static string? CreateStrainRelationship(this Mushroom mushroom)
    {
        return
            mushroom.Strain is null
                ? null
                : $@"
                    MATCH 
                        (x:{mushroom.Tags[0]} {{ Name: '{mushroom.Name}'   }}), 
                        (s:Strain             {{ Name: '{mushroom.Strain}' }})
                    MERGE
                        (x)-[r:HAS_STRAIN]->(s)
                    RETURN r
                ";
    }

    public static string? CreateParentRelationship(this Mushroom mushroom)
    {
        return
            mushroom.Parent is null
                ? null
                : $@"
                      MATCH 
                          (c:{mushroom.Tags[0]} {{ Name: '{mushroom.Name}' }}), 
                          (p:{mushroom.ParentType} {{ Name: '{mushroom.Parent}' }})
                      MERGE
                          (c)-[r:HAS_PARENT]->(p)
                      RETURN r
                  ";
    }

    public static string? CreateChildRelationship(this Mushroom mushroom)
    {
        return 
            mushroom.Child is null 
                ? null 
                : $@"
                    MATCH 
                        (p:{mushroom.Tags[0]} {{ Name: '{mushroom.Name}' }}), 
                        (c:{mushroom.ChildType} {{ Name: '{mushroom.Child}' }})
                    MERGE
                        (c)-[r:HAS_PARENT]->(p)
                    RETURN r
                ";
    }

    public static string? CreateLocationRelationship(this Mushroom mushroom)
    {
        return
            mushroom.Location is null
                ? null
                : $@"
                    MATCH 
                        (x:{mushroom.Tags[0]}  {{ Name: '{mushroom.Name}'     }}), 
                        (l:Location            {{ Name: '{mushroom.Location}' }})
                    MERGE
                        (x)-[r:STORED_IN]->(l)
                    RETURN r
                ";
    }

    public static string CreateNodeLabels(this Mushroom mushroom) => 
        $@"
            MATCH (x:{mushroom.Tags[0]} {{ Name: '{mushroom.Name}' }})
            SET x:{string.Join(":", mushroom.Tags)}
            RETURN x
        ";
    
    public static string CreateCreatedRelationship(this Mushroom mushroom) =>
        $@"
            MATCH 
                (x:{mushroom.Tags[0]} {{ Name: '{mushroom.Name}'      }}),
                (u:User               {{ Name: '{mushroom.CreatedBy}' }})
            MERGE
                (u)-[r:CREATED]->(x)
            RETURN r
        ";

    public static string CreateCreatedOnRelationship(this Mushroom mushroom) =>
        $@"
            MATCH 
                (x:{mushroom.Tags[0]} {{ Name: '{mushroom.Name}' }}), 
                (d:Day                {{ day:   {mushroom.CreatedOn.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {mushroom.CreatedOn.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {mushroom.CreatedOn.Year} }})
            MERGE
                (x)-[r:CREATED_ON]->(d)
            RETURN r
        ";

    public static string? UpdateStatus(this Mushroom mushroom, string elementId)
    {
        return
            mushroom.Successful is null && mushroom.Finished is null
                ? null
                : $@"
                    MATCH 
                        (x:{mushroom.Tags[0]})
                    WHERE 
                        elementId(x) = '{elementId}'
                    REMOVE 
                        x :InProgress:Successful:Failed
                    WITH 
                        x                    
                    SET 
                        x:{mushroom.IsSuccessful()}
                    RETURN 
                        x
                ";
    }
    

    public static string IsSuccessful(this Mushroom mushroom)
    {
        if (mushroom.Finished is null || (bool)!mushroom.Finished) 
            return "InProgress";

        return mushroom.Successful switch
        {
            true  => "Successful",
                _ => "Failed"
        };
    }
}