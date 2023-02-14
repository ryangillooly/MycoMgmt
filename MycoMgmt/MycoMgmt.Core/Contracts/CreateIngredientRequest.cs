namespace MycoMgmt.Core.Contracts;

public class CreateIngredientRequest : CreateRequest
{
    public string Name { get; set; }
    public bool AgentConfigured { get; set; } = false;
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now;
}