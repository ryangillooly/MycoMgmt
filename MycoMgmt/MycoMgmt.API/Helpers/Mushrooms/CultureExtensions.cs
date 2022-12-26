using MycoMgmt.Domain.Models.Mushrooms;

namespace MycoMgmt.Helpers;

public static class CultureExtensions
{
    public static string Create(this Culture culture) =>
        $@"CREATE (x:{culture.Tags[0]} {{ Name:  '{culture.Name}' }}) RETURN x";
    
    
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

    public static string? UpdateVendorRelationship(this Culture culture, string elementId)
    {
        return
            culture.Vendor is null
                ? null
                : $@"
                    MATCH 
                        (x:{culture.Tags[0]})
                    WHERE
                        elementId(x) = '{elementId}'
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
}