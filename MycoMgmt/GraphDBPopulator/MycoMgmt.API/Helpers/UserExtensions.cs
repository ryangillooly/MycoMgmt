using System;
using System.Linq;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.API.Helpers
{
    public static class UserExtensions
    {
        public static void ValidateInput(this User user, string roles, string permissions, string modifiedOn, string modifiedBy)
        {
            if (permissions != null)
                user.Permissions = permissions.Split(',').ToList();

            if (roles != null)
                user.Roles = roles.Split(',').ToList();

            if (modifiedOn != null)
                user.ModifiedOn = DateTime.Parse(modifiedOn);

            if (modifiedBy != null)
                user.ModifiedBy = modifiedBy;
        }
    }
}