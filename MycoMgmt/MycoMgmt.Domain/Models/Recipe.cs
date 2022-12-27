using System;
using System.Collections.Generic;
using MycoMgmt.Domain.Models.UserManagement;

namespace MycoMgmt.Domain.Models
{
    public class Recipe : ModelBase
    {
        public Recipe()
        {
            Tags.Add(EntityTypes.Recipe.ToString());
        }
        
        public string Type { get; set; }
        public string? Description { get; set; }
        public List<string>? Steps { get; set; }
        public string? Notes { get; set; }
        public List<string>? Ingredients { get; set; }
    }
}