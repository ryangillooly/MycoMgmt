// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618

namespace MycoMgmt.Domain.Models.Mushrooms
{
    public class Culture : Mushroom
    {
        public Culture()
        {
            var entityType = EntityTypes.Culture.ToString();
            Tags.Add(entityType);
            EntityType = entityType;
        }
        public string? Vendor { get; set; }
    }
}