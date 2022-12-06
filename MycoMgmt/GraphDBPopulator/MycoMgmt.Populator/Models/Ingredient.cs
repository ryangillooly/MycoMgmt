using System;
using System.Collections.Generic;

namespace MycoMgmt.Populator.Models
{
    public class Ingredient
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Ingredient };
        public string Name { get; set; }
        public Created Created { get; set; }
        public Modified Modified { get; set; }
    }
}