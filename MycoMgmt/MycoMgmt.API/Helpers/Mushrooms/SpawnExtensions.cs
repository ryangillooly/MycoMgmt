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
}