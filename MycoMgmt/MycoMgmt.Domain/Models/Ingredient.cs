using System;
using System.Collections.Generic;
using MycoMgmt.Domain.Models;

namespace MycoMgmt.Domain.Models
{
    public class Ingredient : ModelBase
    {
        public Ingredient()
        {
            Tags.Add(GetType().Name);
        }
    }
}