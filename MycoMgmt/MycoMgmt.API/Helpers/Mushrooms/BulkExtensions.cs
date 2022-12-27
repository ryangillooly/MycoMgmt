using MycoMgmt.Domain.Models.Mushrooms;
using Neo4j.Driver;

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
                                                         Name: '{bulk.Name}' 
                                                         {notes} 
                                                     }}
                                )
                            RETURN x";

        return query;
    }
}