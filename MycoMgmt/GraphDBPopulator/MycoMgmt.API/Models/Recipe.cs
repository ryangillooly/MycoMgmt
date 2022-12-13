using System;
using System.Collections.Generic;
using MycoMgmt.API;
using MycoMgmt.API.Models;

namespace MycoMgmt.API.Models
{
    public class Recipe : ModelBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Recipe };
        public string Type { get; set; }
        public string Description { get; set; }
        public string Steps { get; set; }
        public List<string> Ingredients { get; set; }
    }
}