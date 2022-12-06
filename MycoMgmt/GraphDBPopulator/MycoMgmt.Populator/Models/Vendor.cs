using System;
using System.Collections.Generic;

namespace MycoMgmt.Populator.Models
{
    public class Vendor
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Vendor };
        public string Name { get; set; }
        public Uri URL { get; set; }
        public Created Created { get; set; }
        public Modified Modified { get; set; }
    }
}