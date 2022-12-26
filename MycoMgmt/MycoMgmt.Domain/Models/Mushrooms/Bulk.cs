using System.Collections.Generic;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.Domain.Models.Mushrooms
{
    public class Bulk : Mushroom
    {
        public Bulk()
        {
            Tags.Add(EntityTypes.Bulk.ToString());
        }
        
        public string? Notes { get; set; }
        public string? Recipe { get; set; }
    }
}