using System;
using System.Collections.Generic;
using System.Linq;
#pragma warning disable CS8618

namespace MycoMgmt.Domain.Models.UserManagement
{
    public class User : ModelBase
    {
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.User };
        public string Account { get; set; }
        public List<string>? Permissions { get; set; }
        public List<string>? Roles { get; set; }
    }
}