using System;
using System.Collections.Generic;

namespace MycoMgmt.Populator.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.User };
        public string Name { get; set; }
        public List<string> Permissions { get; set; }
        public List<string> Roles { get; set; }
        public Created Created { get; set; }
        public Modified Modified { get; set; }
    }
}