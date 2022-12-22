using System;
#pragma warning disable CS8618

namespace MycoMgmt.Domain.Models
{
    public class ModelBase
    {
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public string   CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string?   ModifiedBy { get; set; }
    }
}