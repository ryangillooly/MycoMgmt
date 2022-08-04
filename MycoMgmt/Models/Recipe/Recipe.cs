using System;

namespace MycoMgmt.Models
{
    public class Recipe
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string DocType { get; set; }
        public string Description { get; set; }
        public string Steps { get; set; }
        public bool Spawn { get; set; }
        public bool Bulk { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
