using System;
using System.Collections.Generic;
using MycoMgmt.API;
using MycoMgmt.API.Models;
using MycoMgmt.Populator.Models;

namespace MycoMgmt.API.Models
{
    public class Culture
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Culture };
        public string Name { get; set; }
        public CultureTypes Type { get; set; }
        public Strain Strain { get; set; }
        public string? Recipe { get; set; }
        public Locations? Location { get; set; }
        public string? Parent { get; set; }
        public string? Child { get; set; }
        public string? Vendor { get; set; }
        public bool Successful { get; set; }
        public bool Finished { get; set; } = false;
        public Created Created { get; set; }
        public Modified Modified { get; set; }
    }
}