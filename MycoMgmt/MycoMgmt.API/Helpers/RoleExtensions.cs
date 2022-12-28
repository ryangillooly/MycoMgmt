using MycoMgmt.Domain.Models;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.API.Helpers;

public static class RoleExtensions
{
    public static string Create(this IamRole role) => 
        $@"
            CREATE 
                (
                    x:{role.Tags[0]} {{ 
                                         Name: '{role.Name}'
                                      }}
                )
            RETURN 
                x
        ";

    public static string? CreatePermissionRelationship(this Security node)
    {
        return
            node.Permissions is null
            ? null
            : $@"
                MATCH 
                    (x:{node.Tags[0]} {{ Name: '{node.Name}' }}),
                    (p:Permission)
                WHERE
                   p.Name IN ['{string.Join("','", node.Permissions)}']
                CREATE
                    (x)-[rel:HAS]->(p)
                RETURN 
                    rel
              ";
    }
    
    public static string? UpdatePermissionRelationship(this Security node)
    {
        return
            node.Permissions is null
                ? null
                : $@"
                    MATCH 
                        (c:{node.Tags[0]})
                    WHERE
                        elementId(c) = '{node.ElementId}'
                    OPTIONAL MATCH
                        (c)-[r:HAS_PARENT]->(p)
                    DELETE
                        r
                    WITH
                        c
                    MATCH 
                        (p:Permission)
                    WHERE
                        p.Name IN ['{string.Join("','", node.Permissions)}']
                    CREATE 
                        (c)-[r:HAS_PARENT]->(p) 
                    RETURN 
                        r
                ";
    }
    
    public static string? UpdatePermissions(this IamRole role)
    {
        return
            role.Permissions is null
                ? null
                : $@"
                        MATCH 
                            (x:{role.Tags[0]})
                        WHERE
                            elementId(x) = '{role.ElementId}'
                        OPTIONAL MATCH
                            (x)-[r:HAS]->(p)
                        DELETE 
                            r
                        WITH
                            x
                        MATCH
                            (p:Permission)
                        WHERE
                            p.Name IN ['{string.Join("','", role.Permissions)}']
                        CREATE
                            (x)-[r:HAS]->(p)
                        RETURN 
                            r
                  ";
    }
}