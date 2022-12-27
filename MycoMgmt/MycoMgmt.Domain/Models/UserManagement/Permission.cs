using System;
using System.Collections.Generic;
#pragma warning disable CS8618

namespace MycoMgmt.Domain.Models.UserManagement
{
    public class Permission : ModelBase
    {
        public Permission()
        {
            Tags.Add(EntityTypes.Permission.ToString());
        }
    }
}