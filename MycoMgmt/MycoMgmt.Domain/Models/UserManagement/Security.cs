namespace MycoMgmt.Domain.Models.UserManagement;

public class Security : ModelBase
{
    public List<string>? Permissions { get; set; }
}