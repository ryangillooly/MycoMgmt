using MycoMgmt.Domain.Models.Mushrooms;

namespace MycoMgmt.Helpers;

public static class FruitExtensions
{
    public static string Create(this Fruit fruit)
    {
        var additionalData = "";
        
        if (fruit.WetWeight != null)
            additionalData += $",WetWeight: {fruit.WetWeight}";
        
        if (fruit.DryWeight != null)
            additionalData += $",DryWeight: {fruit.DryWeight}";
        
        if (fruit.Notes != null)
            additionalData += $",Notes: '{fruit.Notes}'";
        
        var query = $@"CREATE 
                                (
                                    x:{fruit.Tags[0]} {{ 
                                                         Name: '{fruit.Name}' 
                                                         {additionalData} 
                                                      }}
                                )
                            RETURN x";

        return query;
    }
}