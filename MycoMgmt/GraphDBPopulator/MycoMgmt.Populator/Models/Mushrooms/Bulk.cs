using System;
using System.Collections.Generic;

namespace MycoMgmt.Populator.Models
{
    public class Bulk
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Bulk };
        public string Name { get; set; }
        public string Notes { get; set; }
        public string Recipe { get; set; }
        public string Location { get; set; }
        public string Parent { get; set; }
        public string Child { get; set; }
        public bool Successful { get; set; } = false;
        public bool Finished { get; set; } = false;
        public Created Created { get; set; }
        public Modified Modified { get; set; }
    }
}