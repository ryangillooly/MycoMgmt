using System;

namespace MycoMgmt.Models
{
    public class Fruit
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        public string SourceType { get; set; }
        public string Location { get; set; }
        public bool Finished { get; set; }
        public bool Failure { get; set; }
        public string DocType { get; set; }
        public int WetWeight { get; set; }
        public int DryWeight { get; set; }
        public DateTime HarvestDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime FinishedDate { get; set; }
        public DateTime FailureDate { get; set; }
    }
}
