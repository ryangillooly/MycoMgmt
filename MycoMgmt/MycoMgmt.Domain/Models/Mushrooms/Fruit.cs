using System.Collections.Generic;

namespace MycoMgmt.Domain.Models.Mushrooms
{
    public class Fruit : Mushroom
    {
        public Fruit()
        {
            Tags.Add(EntityTypes.Fruits.ToString());
        }
        
        public decimal WetWeight { get; set; }
        public decimal DryWeight { get; set; }
        public string? Notes { get; set; }
    }
}