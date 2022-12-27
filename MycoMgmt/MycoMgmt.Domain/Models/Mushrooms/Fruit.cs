using System.Collections.Generic;

namespace MycoMgmt.Domain.Models.Mushrooms
{
    public class Fruit : Mushroom
    {
        public Fruit()
        {
            Tags.Add(EntityTypes.Fruit.ToString());
        }
        
        public decimal? WetWeight { get; set; }
        public decimal? DryWeight { get; set; }
    }
}