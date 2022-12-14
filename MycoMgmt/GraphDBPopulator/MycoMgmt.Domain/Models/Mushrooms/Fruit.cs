using System.Collections.Generic;

namespace MycoMgmt.Domain.Models.Mushrooms
{
    public class Fruit : Mushroom
    {
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Fruits };
        public decimal WetWeight { get; set; }
        public decimal DryWeight { get; set; }
        public string Notes { get; set; }
    }
}