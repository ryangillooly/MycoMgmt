using System;
using System.Collections.Generic;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.Domain.Models
{
    public class Location : ModelBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Location };
        public bool AgentConfigured { get; set; } = false;
    }
}