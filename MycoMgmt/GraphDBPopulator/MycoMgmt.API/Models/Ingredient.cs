﻿using System;
using System.Collections.Generic;
using MycoMgmt.API;
using MycoMgmt.API.Models;

namespace MycoMgmt.API.Models
{
    public class Ingredient : ModelBase
    {
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Ingredient };
    }
}