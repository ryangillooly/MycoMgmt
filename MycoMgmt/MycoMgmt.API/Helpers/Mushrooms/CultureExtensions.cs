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
                                                         Name: '{culture.Name}' 
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
}