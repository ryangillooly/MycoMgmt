using System;
using System.Collections.Generic;
using MycoMgmt.Core.Models.UserManagement;

namespace MycoMgmt.Core.Models
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
                                                         Name: '{Name}',
                                                         Id: '{Id}'
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
                        x.Id = '{Id}'                  
                    SET 
                        x.Url = '{Url}'
                    RETURN 
                        x
                ";
        }
    }
}