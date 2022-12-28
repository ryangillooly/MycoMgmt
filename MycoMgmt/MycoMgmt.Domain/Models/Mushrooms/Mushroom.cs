namespace MycoMgmt.Domain.Models.Mushrooms
{
    public class Mushroom : ModelBase
    {
        public string? Strain { get; set; }
        public string? Location { get; set; }
        public string? Parent { get; set; }
        public string? ParentType { get; set; }
        public string? Child { get; set; }
        public string? ChildType { get; set; }
        public string? Status { get; set; }
        public bool? Successful { get; set; }
        public bool? Finished { get; set; }
        public DateTime? FinishedOn { get; set; }
        public DateTime? InoculatedOn { get; set; }
        public string? InoculatedBy { get; set; }
        public string? Recipe { get; set; }
    }
}