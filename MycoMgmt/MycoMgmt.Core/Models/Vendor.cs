using System;
using System.Collections.Generic;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.Domain.Models
{
    public class Vendor : ModelBase
    {
        public string? Url { get; set; }
        
        
        // Create 
        public override string CreateNode()
        {
            var additionalData = "";
        
            if (Notes != null)
                additionalData += $",Notes: '{Notes}'";
        
            if (Url != null)
                additionalData += $",Url: '{Url}'";
        
            var query = $@"CREATE 
                                (
                                    x:{EntityType} {{ 
                                                         Name: '{Name}' 
                                                         {additionalData} 
                                                      }}
                                )
                            RETURN x";

            return query;
        }
        
        // Update
        public override List<string> UpdateQueryList()
        {
            var queryList = new List<string>
            {
                UpdateName(),
                UpdateNotes(),
                UpdateUrl(),
                UpdateModifiedOnRelationship(),
                UpdateModifiedRelationship()
            };

            queryList.RemoveAll(item => item is null);
            return queryList;
        }
        
        public string? UpdateUrl()
        {
            return
                Url is null
                    ? null
                    : $@"
                    MATCH 
                        (x:{EntityType})
                    WHERE 
                        elementId(x) = '{ElementId}'                  
                    SET 
                        x.Url = '{Url}'
                    RETURN 
                        x
                ";
        }
    }
}