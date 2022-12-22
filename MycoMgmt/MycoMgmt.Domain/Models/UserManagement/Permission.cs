using System;
using System.Collections.Generic;
#pragma warning disable CS8618

namespace MycoMgmt.Domain.Models.UserManagement
{
    public class Permission
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Permission };
        public string Name { get; set; }
    }
}