using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace MycoMgmt.Domain.Models.Mushrooms
{
    public class Fruit : Mushroom
    {
        public Fruit()
        {
            var entityType = EntityTypes.Fruit.ToString();
            Tags.Add(entityType);
            EntityType = entityType;
        }
        
        public decimal? WetWeight { get; set; }
        public decimal? DryWeight { get; set; }
    }
}