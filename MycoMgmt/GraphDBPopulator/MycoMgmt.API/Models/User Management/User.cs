using System.Collections.Generic;

namespace MycoMgmt.API.Models.User_Management
{
    public class User : ModelBase
    {
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.User };
        public List<string>? Permissions { get; set; }
        public List<string>? Roles { get; set; }
    }
}