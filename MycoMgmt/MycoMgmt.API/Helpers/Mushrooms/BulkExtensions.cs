using MycoMgmt.API.Helpers;
using MycoMgmt.Domain.Models.Mushrooms;

namespace MycoMgmt.Helpers;

public static class BulkExtensions
{
    public static string Create(this Bulk bulk)
    {
        var notes = "";
        
        if (bulk.Notes != null)
            notes = $",Notes: '{bulk.Notes}'";

        var query = $@"CREATE 
                                (
                                    x:{bulk.Tags[0]} {{ 
                                                         Name:       '{bulk.Name}',
                                                         EntityType: '{bulk.EntityType}',
                                                         Status:     '{bulk.IsSuccessful()}'
                                                         {notes} 
                                                     }}
                                )
                            RETURN x";

        return query;
    }
}