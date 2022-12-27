using MycoMgmt.Domain.Models;

namespace MycoMgmt.API.Helpers;

public static class LocationExtensions
{
    public static string Create(this Location location)
    {
        var additionalData = "";
        
        if (location.AgentConfigured != null)
            additionalData += $",AgentConfigured: '{location.AgentConfigured}'";

        var query = $@"CREATE 
                                (
                                    x:{location.Tags[0]} {{ 
                                                         Name: '{location.Name}'
                                                         {additionalData} 
                                                      }}
                                )
                            RETURN x";

        return query;
    }
}