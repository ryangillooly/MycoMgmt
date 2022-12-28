using System;
using System.Collections.Generic;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.Domain.Models
{
    public class Location : ModelBase
    {
        public Location()
        {
            Tags.Add(EntityTypes.Location.ToString());
        }
        public bool? AgentConfigured { get; set; }
    }
}