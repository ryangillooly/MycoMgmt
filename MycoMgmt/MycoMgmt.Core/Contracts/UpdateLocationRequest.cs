namespace MycoMgmt.Core.Contracts;

public class UpdateLocationRequest : UpdateRequest
{
    public string? Name { get; set; }
    public bool? AgentConfigured { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; } = DateTime.Now;
}