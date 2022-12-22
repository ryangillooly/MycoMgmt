using System.Collections.Generic;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.Domain.Models.Mushrooms
{
    public class Bulk : Mushroom
    {
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Bulk };
        public string? Notes { get; set; }
        public string? Recipe { get; set; }
    }
}