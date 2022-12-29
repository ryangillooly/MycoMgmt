#pragma warning disable CS8618

namespace MycoMgmt.Domain.Models.Mushrooms
{
    public class Spawn : Mushroom
    {   
        public Spawn()
        {
            var entityType = EntityTypes.Spawn.ToString();
            Tags.Add(entityType);
            EntityType = entityType;
        }
    }
}