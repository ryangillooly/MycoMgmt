using System.Collections.Generic;

namespace MycoMgmt.Domain.Models.Mushrooms
{
    public class Culture : Mushroom
    {
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Culture };
        public string Type { get; set; }
        public string Strain { get; set; }
        public string? Recipe { get; set; }
        public string? Vendor { get; set; }
    }
}