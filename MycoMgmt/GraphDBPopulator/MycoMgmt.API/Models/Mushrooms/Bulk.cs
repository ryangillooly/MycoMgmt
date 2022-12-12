﻿using System.Collections.Generic;

namespace MycoMgmt.API.Models.Mushrooms
{
    public class Bulk : ModelBase
    {
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Bulk };
        public string Notes { get; set; }
        public string Recipe { get; set; }
        public string Location { get; set; }
        public string Parent { get; set; }
        public string Child { get; set; }
        public bool Successful { get; set; } = false;
        public bool Finished { get; set; } = false;
    }
}