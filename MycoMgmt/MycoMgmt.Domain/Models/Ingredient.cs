using System;
using System.Collections.Generic;
using MycoMgmt.Domain.Models;

namespace MycoMgmt.Domain.Models
{
    public class Ingredient : ModelBase
    {
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Ingredient };
    }
}