namespace MycoMgmt.Domain.Models.UserManagement;

public class Security : ModelBase
{
    public List<string>? Permissions { get; set; }
    
    public string? CreatePermissionRelationship()
    {
        return
            Permissions is null
                ? null
                : $@"
            MATCH 
                (x:{EntityType} {{ Name: '{Name}' }}),
                (p:Permission)
            WHERE
               p.Name IN ['{string.Join("','", Permissions)}']
            CREATE
                (x)-[rel:HAS]->(p)
            RETURN 
                rel
          ";
    }
    
    public string? UpdatePermissions()
    {
        return
            Permissions is null
                ? null
                : $@"
                        MATCH 
                            (x:{EntityType})
                        WHERE
                            elementId(x) = '{ElementId}'
                        OPTIONAL MATCH
                            (x)-[r:HAS]->(p)
                        DELETE 
                            r
                        WITH
                            x
                        MATCH
                            (p:Permission)
                        WHERE
                            p.Name IN ['{string.Join("','", Permissions)}']
                        CREATE
                            (x)-[r:HAS]->(p)
                        RETURN 
                            r
                  ";
    }
    
    public string? UpdatePermissionRelationship()
    {
        return
            Permissions is null
                ? null
                : $@"
                    MATCH 
                        (c:{EntityType})
                    WHERE
                        elementId(c) = '{ElementId}'
                    OPTIONAL MATCH
                        (c)-[r:HAS_PARENT]->(p)
                    DELETE
                        r
                    WITH
                        c
                    MATCH 
                        (p:Permission)
                    WHERE
                        p.Name IN ['{string.Join("','", Permissions)}']
                    CREATE 
                        (c)-[r:HAS_PARENT]->(p) 
                    RETURN 
                        r
                ";
    }
    
}