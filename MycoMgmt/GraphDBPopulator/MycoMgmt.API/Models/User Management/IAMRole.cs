using System;
using System.Collections.Generic;
using MycoMgmt.API.Models;

namespace MycoMgmt.Populator.Models
{
    public class IAMRole
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.IamRole };
        public string Name { get; set; }
        public List<string> Permissions { get; set; }
    }
}