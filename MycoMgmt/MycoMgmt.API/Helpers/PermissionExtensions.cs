using MycoMgmt.API.Repositories;
using MycoMgmt.Domain.Models;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.API.Helpers;

public static class PermissionExtensions
{
    public static string Create(this Permission permission)
    {
        var query = $@"
                            CREATE 
                            (
                                x:{permission.Tags[0]} {{
                                                          Name: '{permission.Name}'
                                                       }}        
                            ) 
                            RETURN x
                         ";

        return query;
    }
}