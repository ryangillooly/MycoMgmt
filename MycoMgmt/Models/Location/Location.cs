using System;

namespace MycoMgmt.Models
{
    public class Location
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string DocType { get; set; }
        public bool AgentConfigurd { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
