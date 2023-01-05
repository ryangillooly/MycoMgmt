using MycoMgmt.Domain.Models.Mushrooms;

namespace MycoMgmt.API.Models;

public class CultureParameters : Culture
{
    public string   Name      { get; set; }
    public string   Type      { get; set; }
    public string   Strain    { get; set; }
    public DateTime CreatedOn { get; set; }
    public string   CreatedBy { get; set; }
    public int?     count     { get; set;} = 1;
}