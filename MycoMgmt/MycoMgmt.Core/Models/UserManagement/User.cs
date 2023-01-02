using System;
using System.Collections.Generic;
using System.Linq;
#pragma warning disable CS8618

namespace MycoMgmt.Domain.Models.UserManagement
{
    public class User : Security
    {
        public string? Account { get; set; }
        public List<string>? Roles { get; set; }
        
        
        public override List<string> CreateQueryList()
        {
            var queryList = new List<string>
            {
                CreateNode(),
                CreateAccountRelationship(),
                CreateRoleRelationship(),
                CreatePermissionRelationship(),
                CreateCreatedRelationship(),
                CreateCreatedOnRelationship()
            };

            queryList.RemoveAll(item => item is null);
            return queryList;
        }
        
        // Update
        public override List<string> UpdateQueryList()
        {
            var queryList = new List<string>
            {
                UpdateName(),
                UpdateAccountRelationship(),
                UpdateRoleRelationship(),
                UpdatePermissionRelationship(),
                UpdateModifiedOnRelationship(),
                UpdateModifiedRelationship(),
            };

            queryList.RemoveAll(item => item is null);
            return queryList;
        }
        
        public string? CreateAccountRelationship() =>
            $@"
            MATCH 
                (u:{EntityType} {{ Name: '{Name}' }}),
                (a:Account      {{ Name: '{Account}' }})
            MERGE
                (a)-[r:HAS]->(u)
            RETURN 
                r
        ";

        public string? CreateRoleRelationship()
        {
            return
                Roles is null
                    ? null
                    : $@"
                        MATCH
                            (u:{EntityType} {{ Name: '{Name}' }}),
                            (r:IAMRole}})
                        WHERE
                            r.Name IN ['{string.Join("','", Roles)}']
                        MERGE
                            (u)-[rel:HAS]->(r)
                        RETURN 
                            r
                  ";
        }

        public string? UpdateAccountRelationship() =>
            $@"
            MATCH 
                (u:{EntityType} {{ Name: '{Name}' }}),
                (a:Account      {{ Name: '{Account}' }})
            MERGE
                (a)-[r:HAS]->(u)
            RETURN 
                r
        ";

        public string? UpdateRoleRelationship()
        {
            return
                Roles is null
                    ? null
                    : $@"
                    MATCH
                        (u:{EntityType} {{ Name: '{Name}' }}),
                        (r:IAMRole}})
                    WHERE
                        r.Name IN ['{string.Join("','", Roles)}']
                    MERGE
                        (u)-[rel:HAS]->(r)
                    RETURN 
                        r
                  ";
        }
    }
}