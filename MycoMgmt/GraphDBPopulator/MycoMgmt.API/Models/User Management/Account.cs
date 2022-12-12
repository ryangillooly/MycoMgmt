using System;
using System.Collections.Generic;

namespace MycoMgmt.API.Models.User_Management
{
    public class Account : ModelBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Account };
        public List<Users> Users { get; set; }
    }
}