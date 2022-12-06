using System;
using System.Collections.Generic;
using MycoMgmt.API.Models;

namespace MycoMgmt.Populator.Models
{
    public class Permission
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Permission };
        public string Name { get; set; }
    }
}