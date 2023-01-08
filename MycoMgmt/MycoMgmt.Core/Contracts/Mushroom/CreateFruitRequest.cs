namespace MycoMgmt.Domain.Contracts.Mushroom;

public class CreateFruitRequest 
{
    public string?      Name { get; set; }
    public string?     Type { get; set; }
    public string?     Notes { get; set; }
    public decimal     WetWeight { get; set; }
    public decimal     DryWeight { get; set; }
    public string?     Strain { get; set; }
    public string?     Location { get; set; }
    public string?     Parent { get; set; }
    public string?     ParentType { get; set; }
    public string?     Children { get; set; }
    public string?     ChildType { get; set; }
    public bool?       Successful { get; set; }
    public bool        Finished { get; set; }
    public bool?       Purchased { get; set; }
    public string      HarvestedBy { get; set; }
    public DateTime    HarvestedOn { get; set; }
    public DateTime?   CreatedOn { get; set; }
    public string?     CreatedBy { get; set; }
    public DateTime?   ModifiedOn { get; set; }
    public string?     ModifiedBy { get; set; }
    public string?     Vendor { get; set; }
    public int?        Count     { get; set; } = 1;
}