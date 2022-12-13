using System;
using System.Collections.Generic;

namespace MycoMgmt.API.Models.Mushrooms
{
    public class Spawn : ModelBase
    {
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Spawn };
        public string Type { get; set; }
        public string Notes { get; set; }
        public string Recipe { get; set; }
        public string Location { get; set; }
        public Guid Parent { get; set; }
        public Guid Child { get; set; }
        public bool Successful { get; set; } = false;
        public bool Finished { get; set; } = false;
    }
}