using System;
using System.Collections.Generic;

namespace MycoMgmt.Populator.Models
{
    public class Account
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Account };
        public string Name { get; set; }
        public List<Users> Users { get; set; }
        public Created Created { get; set; }
        public Modified Modified { get; set; }
    }
}