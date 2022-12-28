// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
using MycoMgmt.Domain.Models.Mushrooms;

namespace MycoMgmt.API.Helpers;

public static class MushroomExtensions
{
    public static string? UpdateParentRelationship(this Mushroom mushroom)
    {
        return
            mushroom.Parent is null
                ? null
                : $@"
                    MATCH 
                        (c:{mushroom.Tags[0]})
                    WHERE
                        elementId(c) = '{mushroom.ElementId}'
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

    public static string? UpdateChildRelationship(this Mushroom mushroom)
    {
        return
            mushroom.Children is null
                ? null
                : $@"
                    MATCH 
                        (p:{mushroom.Tags[0]})
                    WHERE
                        elementId(p) = '{mushroom.ElementId}'
                    OPTIONAL MATCH
                        (c)-[r:HAS_PARENT]->(p)
                    DELETE
                        r
                    WITH
                        p
                    MATCH 
                        (c:{mushroom.ChildType} {{Name: '{mushroom.Children}' }})
                    MERGE 
                        (c)-[r:HAS_PARENT]->(p) 
                    RETURN 
                        r
                ";
    }

    public static string? UpdateStrainRelationship(this Mushroom mushroom)
    {
        return
            mushroom.Strain is null
                ? null
                : $@"
                    MATCH 
                        (x:{mushroom.Tags[0]})
                    WHERE
                        elementId(x) = '{mushroom.ElementId}'
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
    
    public static string? UpdateLocationRelationship(this Mushroom mushroom)
    {
        return
            mushroom.Location is null
                ? null
                : $@"
                    MATCH 
                        (x:{mushroom.Tags[0]})
                    WHERE
                        elementId(x) = '{mushroom.ElementId}'
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
    
    public static string? UpdateRecipeRelationship(this Mushroom mushroom)
    {
        return
            mushroom.Recipe is null
                ? null
                : $@"
                    MATCH 
                        (c:{mushroom.Tags[0]})
                    WHERE
                        elementId(c) = '{mushroom.ElementId}'
                    OPTIONAL MATCH
                        (c)-[r:CREATED_USING]->(:Recipe)
                    DELETE 
                        r
                    WITH
                        c
                    MATCH
                        (recipe:Recipe {{ Name: '{mushroom.Recipe}' }})
                    MERGE
                        (c)-[r:CREATED_USING]->(recipe)
                    RETURN 
                        r
                ";
    }
    
    public static string? UpdateStatusLabel(this Mushroom mushroom)
    {
        return
            mushroom.Successful is null && mushroom.Finished is null
                ? null
                : $@"
                    MATCH 
                        (x:{mushroom.Tags[0]})
                    WHERE 
                        elementId(x) = '{mushroom.ElementId}'
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
    
    public static string? UpdateStatus(this Mushroom mushroom)
    {
        return
            mushroom.Successful is null && mushroom.Finished is null
                ? null
                : $@"
                    MATCH 
                        (x:{mushroom.EntityType})
                    WHERE 
                        elementId(x) = '{mushroom.ElementId}'
                    SET 
                        x {{ Status: '{mushroom.IsSuccessful()}' }}
                    RETURN 
                        x
                ";
    }
    
    public static string? UpdateInoculatedRelationship(this Mushroom mushroom)
    {
        return
            mushroom.InoculatedBy is null
                ? null
                : $@"
                    MATCH 
                        (x:{mushroom.Tags[0]})
                    WHERE
                        elementId(x) = '{mushroom.ElementId}'
                    OPTIONAL MATCH
                        (u:User)-[r:INOCULATED]->(x)
                    DELETE 
                        r
                    WITH
                        x
                    MATCH
                        (u:User {{ Name: '{mushroom.InoculatedBy}' }})
                    MERGE
                        (u)-[r:INOCULATED]->(x)
                    RETURN 
                        r
                ";
    }

    public static string? UpdateInoculatedOnRelationship(this Mushroom mushroom)
    {
        return
            mushroom.InoculatedOn is null
                ? null
                : $@"
                    MATCH 
                        (x:{mushroom.Tags[0]})
                    WHERE
                        elementId(x) = '{mushroom.ElementId}'
                    OPTIONAL MATCH
                        (x)-[r:INOCULATED_ON]->(d)
                    DELETE 
                        r
                    WITH
                        x
                    MATCH
                        (d:Day {{ day: {mushroom.InoculatedOn.Value.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {mushroom.InoculatedOn.Value.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {mushroom.InoculatedOn.Value.Year} }})
                    MERGE
                        (x)-[r:INOCULATED_ON]->(d)
                    RETURN 
                        r
                ";
    }
    
    public static string? UpdateFinishedOnRelationship(this Mushroom mushroom)
    {
        return
            mushroom.FinishedOn is null
                ? null
                : $@"
                    MATCH 
                        (x:{mushroom.Tags[0]})
                    WHERE
                        elementId(x) = '{mushroom.ElementId}'
                    OPTIONAL MATCH
                        (x)-[r:FINISHED_ON]->(d)
                    DELETE 
                        r
                    WITH
                        x
                    MATCH
                        (d:Day {{ day: {mushroom.FinishedOn.Value.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {mushroom.FinishedOn.Value.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {mushroom.FinishedOn.Value.Year} }})
                    MERGE
                        (x)-[r:FINISHED_ON]->(d)
                    RETURN 
                        r
                ";
    }
    
    public static string? CreateFinishedOnRelationship(this Mushroom mushroom)
    {
        return
            mushroom.FinishedOn is null
                ? null
                : $@"
                        MATCH 
                            (x:{mushroom.Tags[0]} {{ Name: '{mushroom.Name}' }}), 
                            (d:Day                {{ day:   {mushroom.FinishedOn.Value.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {mushroom.FinishedOn.Value.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {mushroom.FinishedOn.Value.Year} }})
                        CREATE
                            (x)-[r:FINISHED_ON]->(d)
                        RETURN r
                  ";
    }

    public static string? CreateRecipeRelationship(this Mushroom mushroom)
    {
        return
            mushroom.Recipe is null
                ? null
                : $@"
                    MATCH 
                        (c:{mushroom.Tags[0]} {{ Name: '{mushroom.Name}'   }}),
                        (recipe:Recipe       {{ Name: '{mushroom.Recipe}' }})
                    CREATE
                        (c)-[r:CREATED_USING]->(recipe)
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
                    CREATE
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
                      CREATE
                          (c)-[r:HAS_PARENT]->(p)
                      RETURN r
                  ";
    }
    
    public static string? CreateInoculatedOnRelationship(this Mushroom mushroom)
    {
        return
            mushroom.InoculatedOn is null
                ? null
                : $@"
                        MATCH 
                            (x:{mushroom.Tags[0]} {{ Name: '{mushroom.Name}' }}), 
                            (d:Day                {{ day:   {mushroom.InoculatedOn.Value.Day} }})<-[:HAS_DAY]-(m:Month {{ month: {mushroom.InoculatedOn.Value.Month} }})<-[:HAS_MONTH]-(y:Year {{ year: {mushroom.InoculatedOn.Value.Year} }})
                        CREATE
                            (x)-[r:INOCULATED_ON]->(d)
                        RETURN r
                  ";
    }
    
    public static string? CreateInoculatedRelationship(this Mushroom mushroom)
    {
        return
            mushroom.InoculatedBy is null
                ? null
                : $@"
                      MATCH 
                          (x:{mushroom.Tags[0]} {{ Name: '{mushroom.Name}'         }}),
                          (u:User               {{ Name: '{mushroom.InoculatedBy}' }})
                      CREATE
                          (u)-[r:INOCULATED]->(x)
                      RETURN r
                  ";
    }
    
    public static string? CreateChildRelationship(this Mushroom mushroom)
    {
        return 
            mushroom.Children is null 
                ? null 
                : $@"
                    MATCH 
                        (p:{mushroom.Tags[0]} {{ Name: '{mushroom.Name}' }}), 
                        (c:{mushroom.ChildType} {{ Name: '{mushroom.Children}' }})
                    CREATE
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
                    CREATE
                        (x)-[r:STORED_IN]->(l)
                    RETURN r
                ";
    }

    public static string? CreateNodeLabels(this Mushroom mushroom)
    {
        return
            $@"
                MATCH (x:{mushroom.Tags[0]} {{ Name: '{mushroom.Name}' }})
                SET x:{string.Join(":", mushroom.Tags)}
                RETURN x
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