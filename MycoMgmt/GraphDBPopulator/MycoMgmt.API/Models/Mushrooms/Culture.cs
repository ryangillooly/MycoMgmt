using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using MycoMgmt.API;
using MycoMgmt.API.Models;
using MycoMgmt.Populator.Models;

namespace MycoMgmt.API.Models
{
    public class Culture : ModelBase
    {
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Culture };
        public CultureTypes Type { get; set; }
        public Strain Strain { get; set; }
        public string? Recipe { get; set; }
        public Locations? Location { get; set; }
        public string? Parent { get; set; }
        public string? Child { get; set; }
        public string? Vendor { get; set; }
        public bool? Successful { get; set; }
        public bool Finished { get; set; } = false;
    }
}