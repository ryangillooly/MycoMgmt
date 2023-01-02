using System;
using System.Collections.Generic;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.Domain.Models
{
    public class Location : ModelBase
    {
        public bool? AgentConfigured { get; set; }
        
        
        // Create
        public override string CreateNode()
        {
            var additionalData = "";
        
            if (AgentConfigured != null)
                additionalData += $",AgentConfigured: '{AgentConfigured}'";

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
                UpdateAgentConfigured(),
                UpdateModifiedOnRelationship(),
                UpdateModifiedRelationship()
            };

            queryList.RemoveAll(item => item is null);
            return queryList;
        }

        private string? UpdateAgentConfigured()
        {
            return
                AgentConfigured is null
                    ? null
                    : $@"
                    MATCH 
                        (x:{EntityType}) 
                    WHERE 
                        x.Id = '{Id}' 
                    SET 
                        x.AgentConfigured = '{AgentConfigured}' 
                    RETURN 
                        x 
                  ";
        }
    }
}