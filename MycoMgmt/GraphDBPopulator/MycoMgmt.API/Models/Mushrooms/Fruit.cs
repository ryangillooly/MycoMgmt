using System.Collections.Generic;

namespace MycoMgmt.API.Models.Mushrooms
{
    public class Fruit : ModelBase
    {
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Fruits };
        public decimal WetWeight { get; set; }
        public decimal DryWeight { get; set; }
        public string Notes { get; set; }
        public string Location { get; set; }
        public string Parent { get; set; }
        public string Child { get; set; }
        public bool Successful { get; set; } = false;
        public bool Finished { get; set; } = false;
    }
}