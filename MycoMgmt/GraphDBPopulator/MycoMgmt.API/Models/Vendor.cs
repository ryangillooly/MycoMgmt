using System;
using System.Collections.Generic;
using MycoMgmt.API;
using MycoMgmt.API.Models;

namespace MycoMgmt.API.Models
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