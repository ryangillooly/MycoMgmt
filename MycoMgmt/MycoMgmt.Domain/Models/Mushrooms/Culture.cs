// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618

namespace MycoMgmt.Domain.Models.Mushrooms
{
    public class Culture : Mushroom
    {
        public Culture()
        {
            Tags.Add(EntityTypes.Culture.ToString());
        }
        public string Type { get; set; }
        public string? Vendor { get; set; }
    }
}