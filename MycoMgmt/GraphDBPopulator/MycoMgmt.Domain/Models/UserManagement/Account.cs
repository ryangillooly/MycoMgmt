using System;
using System.Collections.Generic;

namespace MycoMgmt.Domain.Models.UserManagement
{
    public class Account : ModelBase
    {
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Account };
        public List<string> Users { get; set; }
    }
}