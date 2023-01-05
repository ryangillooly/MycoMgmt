namespace MycoMgmt.Domain.Contracts.Mushroom.Culture;

public class CreateCultureRequest 
{
    public Guid Id { get; set; }
    public string? ElementId { get; set; }
    public List<string> Tags { get; set; } = new ();
    public string? EntityType { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Notes { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string?   CreatedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public string?   ModifiedBy { get; set; }
    public string? Strain { get; set; }
    public string? Location { get; set; }
    public string? Parent { get; set; }
    public string? ParentType { get; set; }
    public string? Children { get; set; }
    public string? ChildType { get; set; }
    public string? Status { get; set; }
    public bool? Successful { get; set; }
    public bool? Finished { get; set; }
    public DateTime? FinishedOn { get; set; }
    public DateTime? InoculatedOn { get; set; }
    public string? InoculatedBy { get; set; }
    public string? Recipe { get; set; }
    public bool? Purchased { get; set; }
    public string? Vendor { get; set; }
    
    public int?         Count     { get; set; } = 1;
}