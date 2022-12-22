#pragma warning disable CS8618

namespace MycoMgmt.Domain.Models.Mushrooms
{
    public class Spawn : Mushroom
    {
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Spawn };
        public string Type { get; set; }
        public string? Notes { get; set; }
        public string? Recipe { get; set; }
    }
}