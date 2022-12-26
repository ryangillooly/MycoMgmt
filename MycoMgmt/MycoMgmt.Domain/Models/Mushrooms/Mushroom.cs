﻿namespace MycoMgmt.Domain.Models.Mushrooms
{
    public class Mushroom : ModelBase
    {
        public List<string> Tags { get; set; } = new List<string>();
        public string? Location { get; set; }
        public string? Parent { get; set; }
        public string? Child { get; set; }
        public bool Successful { get; set; }
        public bool? Finished { get; set; } = false;
    }
}