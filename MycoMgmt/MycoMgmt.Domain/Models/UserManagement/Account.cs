﻿using System;
using System.Collections.Generic;

namespace MycoMgmt.Domain.Models.UserManagement
{
    public class Account : ModelBase
    {
        public Account()
        {
            Tags.Add(GetType().Name);
        }
    }
}