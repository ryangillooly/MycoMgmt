using System.Collections.Generic;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.Domain.Models.Mushrooms
{
    public class Bulk : Mushroom
    {
        public Bulk()
        {
            var entityType = EntityTypes.Bulk.ToString();
            Tags.Add(entityType);
            EntityType = entityType;
        }
    }
}