using System.Collections.Generic;

namespace MycoMgmt.Domain.Models.UserManagement
{
    public class User : ModelBase
    {
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.User };
        public string Account { get; set; }
        public List<string>? Permissions { get; set; }
        public List<string>? Roles { get; set; }
    }
}