namespace MycoMgmt.Domain.Contracts.Mushroom;

public class UpdateMushroomRequest 
{
    public string?      Name { get; set; }
    public string?      Type { get; set; }
    public string?      Notes { get; set; }
    public string?      Strain { get; set; }
    public decimal?     WetWeight { get; set; }
    public decimal?     DryWeight { get; set; }
    public string?      Location { get; set; }
    public string?      Parent { get; set; }
    public string?      ParentType { get; set; }
    public string?      Children { get; set; }
    public string?      ChildType { get; set; }
    public bool?        Successful { get; set; }
    public bool?        Finished { get; set; }
    public bool?        Purchased { get; set; }
    public DateTime?    FinishedOn { get; set; }
    public DateTime?    InoculatedOn { get; set; }
    public string?      InoculatedBy { get; set; }
    public string?      HarvestedBy { get; set; }
    public DateTime?    HarvestedOn { get; set; }
    public DateTime?    ModifiedOn { get; set; } = DateTime.Now;
    public string?      ModifiedBy { get; set; }
    public string?      Recipe { get; set; }
    public string?      Vendor { get; set; }
}