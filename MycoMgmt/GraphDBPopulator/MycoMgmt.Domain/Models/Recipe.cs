using System;
using System.Collections.Generic;

namespace MycoMgmt.Domain.Models
{
    public class Recipe : ModelBase
    {
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Recipe };
        public string Type { get; set; }
        public string Description { get; set; }
        public string Steps { get; set; }
        public List<string> Ingredients { get; set; }
    }
}