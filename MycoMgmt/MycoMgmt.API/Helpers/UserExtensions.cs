using System;
using System.Linq;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.API.Helpers
{
    public static class UserExtensions
    {
        public static string Create(this User user) =>
            $@"
                CREATE 
                (
                    x:{user.Tags[0]} {{ 
                                         Name: '{user.Name}'
                                      }}
                )
                RETURN 
                x
            ";
        
        public static string? CreateAccountRelationship(this User user) =>
            $@"
            MATCH 
                (u:{user.Tags[0]} {{ Name: '{user.Name}' }}),
                (a:Account        {{ Name: '{user.Account}' }})
            MERGE
                (a)-[r:HAS]->(u)
            RETURN 
                r
        ";

        public static string? CreateRoleRelationship(this User user)
        {
            return
                user.Roles is null
                    ? null
                    : $@"
                        MATCH
                            (u:{user.Tags[0]} {{ Name: '{user.Name}' }}),
                            (r:IAMRole}})
                        WHERE
                            r.Name IN ['{string.Join("','", user.Roles)}']
                        MERGE
                            (u)-[rel:HAS]->(r)
                        RETURN 
                            r
                  ";
        }

        public static string? UpdateAccountRelationship(this User user) =>
            $@"
            MATCH 
                (u:{user.Tags[0]} {{ Name: '{user.Name}' }}),
                (a:Account        {{ Name: '{user.Account}' }})
            MERGE
                (a)-[r:HAS]->(u)
            RETURN 
                r
        ";

        public static string? UpdateRoleRelationship(this User user)
        {
            return
                user.Roles is null
                ? null
                : $@"
                    MATCH
                        (u:{user.Tags[0]} {{ Name: '{user.Name}' }}),
                        (r:IAMRole}})
                    WHERE
                        r.Name IN ['{string.Join("','", user.Roles)}']
                    MERGE
                        (u)-[rel:HAS]->(r)
                    RETURN 
                        r
                  ";
        }
    }
}