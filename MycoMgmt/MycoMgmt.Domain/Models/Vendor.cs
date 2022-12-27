using System;
using System.Collections.Generic;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.Domain.Models
{
    public class Vendor : ModelBase
    {
        public Vendor()
        {
            Tags.Add(EntityTypes.Vendor.ToString());
        }

        public string? Url { get; set; }
        public string? Notes { get; set; }
    }
}