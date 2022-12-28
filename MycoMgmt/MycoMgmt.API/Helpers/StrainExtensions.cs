using MycoMgmt.API.Repositories;
using MycoMgmt.Domain.Models;

namespace MycoMgmt.API.Helpers;

public static class StrainExtensions
{
    public static string Create(this Strain strain)
    {
        var additionalData = "";
        
        if (strain.Effects != null)
            additionalData += $",Effects: '{strain.Effects}'";
        
        var query = $@"
                            CREATE 
                            (
                                x:{strain.Tags[0]} {{ 
                                                     Name: '{strain.Name}'
                                                     {additionalData} 
                                                  }}
                            )
                            RETURN x
                            ";

        return query;
    }
    
    public static string? UpdateEffects(this Strain strain)
    {
        return
            strain.Name is null
                ? null
                : $@"
                MATCH 
                    (x:{strain.Tags[0]}) 
                WHERE 
                    elementId(x) = '{strain.ElementId}' 
                SET 
                    x.Effects = '{strain.Name}' 
                RETURN 
                    x
              ";
    }
}