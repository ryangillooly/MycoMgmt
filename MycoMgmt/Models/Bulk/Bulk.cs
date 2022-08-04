using System;

namespace MycoMgmt.Models
{
    public class Bulk
    {
        public string Id { get; set; }
        public string Source { get; set; }
        public string SourceType { get; set; }
        public string Location { get; set; }
        public bool Finished { get; set; }
        public bool Failure { get; set; }
        public string DocType { get; set; }
        public string Recipe { get; set; }
        public string Notes { get; set; }
        public string SterilizationNotes { get; set; }
        public DateTime InoculationDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime FinishedDate { get; set; }
        public DateTime FailureDate { get; set; }
    }
}
