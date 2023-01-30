namespace MycoMgmt.Core.Models.DTO;

public class CultureDto
{
    public string    Id { get; set; }
    public string    EntityType { get; set; }
    public string    Name { get; set; }
    public string    Strain { get; set; }
    public string?   Type { get; set; }
    public string?   Notes { get; set; }
    public string?   Location { get; set; }
    public string?   Parent { get; set; }
    public string?   ParentType { get; set; }
    public string?   Children { get; set; }
    public string?   ChildType { get; set; }
    public bool?     Successful { get; set; }
    public bool      Finished { get; set; }
    public bool?     Purchased { get; set; }
    public DateTime? FinishedOn { get; set; }
    public DateTime? InoculatedOn { get; set; }
    public string?   InoculatedBy { get; set; }
    public DateTime  CreatedOn { get; set; }
    public string    CreatedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public string?   ModifiedBy { get; set; }
    public string?   Recipe { get; set; }
    public string?   Vendor { get; set; }
    public int?      Count     { get; set; } = 1;
}
/*
{{Id:           x.Id}},
{{EntityType:   properties(x).EntityType}},
{{Type:         properties(x).Type}},
{{Name:         properties(x).Name}},
{{Notes:        properties(x).Notes}},
{{Strain:       strain}},
{{Status:       properties(x).Status}},
{{Location:     location}},
{{Parent:       properties(parent).Name}},
{{ParentType:   properties(parent).EntityType}},
{{Children:     children}},
{{InoculatedOn: apoc.date.toISO8601(inoculatedDate.epochMillis, 'ms')}},
{{InoculatedBy: inoculatedBy.Name}},
{{CreatedOn:    apoc.date.toISO8601(createdDate.epochMillis, 'ms')}},
{{CreatedBy:    createdBy}},
{{ModifiedOn:   apoc.date.toISO8601(modifiedDate.epochMillis, 'ms')}},        
{{ModifiedBy:   modifiedBy}},
{{FinishedOn:   apoc.date
*/