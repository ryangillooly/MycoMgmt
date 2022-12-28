// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
using MycoMgmt.Domain.Models.Mushrooms;

namespace MycoMgmt.API.Helpers;

public static class MushroomExtensions
{
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
    
    public static string? UpdateRecipeRelationship(this Mushroom mushroom, string elementId)
    {
        return
            mushroom.Recipe is null
                ? null
                : $@"
                    MATCH 
                        (c:{mushroom.Tags[0]})
                    WHERE
                        elementId(c) = '{elementId}'
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

    public static string? UpdateName(this Mushroom mushroom, string elementId)
    {
        return
            mushroom.Name is null
                ? null
                : $@"
                    MATCH 
                        (x:{mushroom.Tags[0]}) 
                    WHERE 
                        elementId(x) = '{elementId}' 
                    SET 
                        x.Name = '{mushroom.Name}' 
                    RETURN s 
                  ";
    }
    
    public static string? UpdateNotes(this Mushroom mushroom, string elementId)
    {
        return
            mushroom.Notes is null
                ? null
                : $@"
                    MATCH 
                        (x:{mushroom.Tags[0]}) 
                    WHERE 
                        elementId(x) = '{elementId}' 
                    SET 
                        x.Notes = '{mushroom.Notes}' 
                    RETURN s 
                  ";
    }
    
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
    
    public static string? CreateRecipeRelationship(this Mushroom mushroom)
    {
        return
            mushroom.Recipe is null
                ? null
                : $@"
                    MATCH 
                        (c:{mushroom.Tags[0]} {{ Name: '{mushroom.Name}'   }}),
                        (recipe:Recipe       {{ Name: '{mushroom.Recipe}' }})
                    MERGE
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

    public static string CreateNodeLabels(this Mushroom mushroom)
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

    public static string SearchByNameQuery(this Mushroom mushroom) =>
        $@"
            MATCH 
                (x:{mushroom.Tags[0]}) 
            WHERE 
                toUpper(x.Name) CONTAINS toUpper('{ mushroom.Name }') 
            RETURN 
                x 
            ORDER BY 
                x.Name 
            LIMIT 
                100
        ";
    
    public static string GetByNameQuery(this Mushroom mushroom) =>
        $@"
            MATCH 
                (x:{mushroom.Tags[0]}) 
            WHERE 
                toUpper(x.Name) = toUpper('{ mushroom.Name }') 
            RETURN 
                x
        ";
    
    public static string GetByIdQuery(this Mushroom mushroom) =>
        $@"
            MATCH 
                (x:{mushroom.Tags[0]}) 
            WHERE 
                elementId(x) = '{mushroom.ElementId}'
            RETURN 
                x
        ";
}