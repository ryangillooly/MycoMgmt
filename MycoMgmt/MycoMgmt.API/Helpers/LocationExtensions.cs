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

    public static string? UpdateAgentConfigured(this Location location)
    {
        return
            location.AgentConfigured is null
                ? null
                : $@"
                    MATCH 
                        (x:{location.Tags[0]}) 
                    WHERE 
                        elementId(x) = '{location.ElementId}' 
                    SET 
                        x.AgentConfigured = '{location.AgentConfigured}' 
                    RETURN 
                        x 
                  ";
    }
}