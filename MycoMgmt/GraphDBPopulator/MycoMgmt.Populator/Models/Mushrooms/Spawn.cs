using System;
using System.Collections.Generic;

namespace MycoMgmt.Populator.Models
{
    public class Spawn
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Spawn };
        public string Name { get; set; }
        public SpawnTypes Type { get; set; }
        public string Notes { get; set; }
        public string Recipe { get; set; }
        public Locations Location { get; set; }
        public Guid Parent { get; set; }
        public Guid Child { get; set; }
        public bool Successful { get; set; } = false;
        public bool Finished { get; set; } = false;
        public Created Created { get; set; }
        public Modified Modified { get; set; }
    }
}