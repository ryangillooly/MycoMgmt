using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Formats.Asn1;

#pragma warning disable CS8618

namespace MycoMgmt.Domain.Models.UserManagement
{
    public class IamRole : Security
    {
        public IamRole()
        {
            Tags.Add(EntityTypes.IamRole.ToString());
        }
        
    }
}