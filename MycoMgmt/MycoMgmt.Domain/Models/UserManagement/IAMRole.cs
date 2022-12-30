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
            Tags.Add(GetType().Name);
            Type = GetType().Name;
        }
     
        
        public override List<string> CreateQueryList()
        {
            var queryList = new List<string>
            {
                CreateNode(),
                CreatePermissionRelationship(),
                CreateCreatedRelationship(),
                CreateCreatedOnRelationship(),
            };

            queryList.RemoveAll(item => item is null);
            return queryList;
        }

        // Update
        public override List<string> UpdateQueryList()
        {
            var queryList = new List<string>
            {
                UpdateName(),
                UpdatePermissions(),
                UpdateModifiedRelationship(),
                UpdateModifiedOnRelationship()
            };

            queryList.RemoveAll(item => item is null);
            return queryList;
        }
    }
}