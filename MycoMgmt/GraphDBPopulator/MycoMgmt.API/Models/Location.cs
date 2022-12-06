using System;
using System.Collections.Generic;
using MycoMgmt.API;
using MycoMgmt.API.Models;

namespace MycoMgmt.API.Models
{
    public class Location
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Location };
        public string Name { get; set; }
        public bool AgentConfigured { get; set; } = false;
        public Created Created { get; set; }
        public Modified Modified { get; set; }
    }
}