#pragma warning disable CS8618

namespace MycoMgmt.Domain.Models.Mushrooms
{
    public class Spawn : Mushroom
    {   
        public Spawn()
        {
            Tags.Add(EntityTypes.Spawn.ToString());
        }
        public string Type { get; set; }
    }
}