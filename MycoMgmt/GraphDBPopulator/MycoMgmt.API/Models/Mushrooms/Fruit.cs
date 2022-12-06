using System;
using System.Collections.Generic;
using MycoMgmt.API;
using MycoMgmt.API.Models;

namespace MycoMgmt.API.Models
{
    public class Fruit
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Fruits };
        public string Name { get; set; }
        public decimal WetWeight { get; set; }
        public decimal DryWeight { get; set; }
        public string Notes { get; set; }
        public string Location { get; set; }
        public string Parent { get; set; }
        public string Child { get; set; }
        public bool Successful { get; set; } = false;
        public bool Finished { get; set; } = false;
        public Created Created { get; set; }
        public Modified Modified { get; set; }
    }
}