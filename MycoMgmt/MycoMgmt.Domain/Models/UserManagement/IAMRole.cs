using System;
using System.Collections.Generic;
#pragma warning disable CS8618

namespace MycoMgmt.Domain.Models.UserManagement
{
    public class IamRole
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.IamRole };
        public string Name { get; set; }
        public List<string> Permissions { get; set; }
    }
}