using System.Collections.Generic;

namespace MycoMgmt.API.Models.Mushrooms
{
    public class Culture : ModelBase
    {
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Culture };
        public CultureTypes Type { get; set; }
        public Strain Strain { get; set; }
        public string? Recipe { get; set; }
        public Locations? Location { get; set; }
        public string? Parent { get; set; }
        public string? Child { get; set; }
        public string? Vendor { get; set; }
        public bool? Successful { get; set; }
        public bool Finished { get; set; } = false;
    }
}