﻿using System;
using System.Collections.Generic;
using System.Linq;
#pragma warning disable CS8618

namespace MycoMgmt.Domain.Models.UserManagement
{
    public class User : ModelBase
    {
        public User()
        {
            Tags.Add(EntityTypes.User.ToString());
        }
        public string Account { get; set; }
        public List<string>? Permissions { get; set; }
        public List<string>? Roles { get; set; }
    }
}