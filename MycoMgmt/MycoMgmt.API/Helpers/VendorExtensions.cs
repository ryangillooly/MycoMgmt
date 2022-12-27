using MycoMgmt.Domain.Models;

namespace MycoMgmt.API.Helpers;

public static class VendorExtensions
{
    public static string Create(this Vendor vendor)
    {
        var additionalData = "";
        
        if (vendor.Notes != null)
            additionalData += $",Notes: '{vendor.Notes}'";
        
        if (vendor.Url != null)
            additionalData += $",Url: '{vendor.Url}'";
        
        var query = $@"CREATE 
                                (
                                    x:{vendor.Tags[0]} {{ 
                                                         Name: '{vendor.Name}' 
                                                         {additionalData} 
                                                      }}
                                )
                            RETURN x";

        return query;
    }
}