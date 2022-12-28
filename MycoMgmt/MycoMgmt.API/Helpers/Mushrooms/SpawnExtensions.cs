using MycoMgmt.Domain.Models.Mushrooms;

namespace MycoMgmt.Helpers;

public static class SpawnExtensions
{
    public static string Create(this Spawn spawn) 
    {
        var additionalData = "";
        
        if (spawn.Notes != null)
            additionalData += $",Notes: '{spawn.Notes}'";
        
        var query = $@"CREATE 
                                (
                                    x:{spawn.Tags[0]} {{ 
                                                         Name: '{spawn.Name}' 
                                                         {additionalData} 
                                                      }}
                                )
                            RETURN x";

        return query;
    }
    
    public static string? UpdateType(this Spawn spawn, string elementId)
    {
        return
            spawn.Type is null
                ? null
                : $@"
                    MATCH 
                        (x:{spawn.Tags[0]}) 
                    WHERE 
                        elementId(x) = '{elementId}' 
                    SET 
                        x.Name = '{spawn.Name}' 
                    RETURN s 
                  ";
    }
    
}