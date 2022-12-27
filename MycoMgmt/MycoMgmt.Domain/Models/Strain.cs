using System;
using System.Collections.Generic;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.Domain.Models
{
    public class Strain : ModelBase
    {
        public Strain()
        {
            Tags.Add(EntityTypes.Strain.ToString());
        }
        public string? Effects { get; set; }
    }
}