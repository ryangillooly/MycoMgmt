using MycoMgmt.Domain.Models;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.API.Helpers;

public static class AccountExtensions
{
    public static string Create(this Account account) => 
        $@"CREATE (x:{account.Tags[0]} {{  Name: '{account.Name}' }}) RETURN x";
}