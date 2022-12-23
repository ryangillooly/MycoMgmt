// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618

namespace MycoMgmt.Domain.Models.Mushrooms
{
    public class Culture : Mushroom
    {
        public List<EntityTypes> Tags { get; set; } = new List<EntityTypes> { EntityTypes.Culture };
        public string Type { get; set; }
        public string Strain { get; set; }
        public string? Recipe { get; set; }
        public string? Vendor { get; set; }
    }

    public static class CultureExtensions
    {
        public static string IsSuccessful(this Culture culture)
        {
            if ((bool)!culture.Finished) 
                return ":InProgress";

            return culture.Successful switch
            {
                true => ":Successful",
                false => ":Failed"
            };
        }
    }
}