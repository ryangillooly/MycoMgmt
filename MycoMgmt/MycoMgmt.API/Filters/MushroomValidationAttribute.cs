using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MycoMgmt.Domain.Models.Mushrooms;

namespace MycoMgmt.API.Filters;

public class MushroomValidationAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var mushroom = context.ActionArguments;

        mushroom.TryGetValue("parent",     out var parent);
        mushroom.TryGetValue("parentType", out var parentType);
        mushroom.TryGetValue("children",   out var children);
        mushroom.TryGetValue("childType",  out var childType);
        mushroom.TryGetValue("purchased",  out var purchased);
        mushroom.TryGetValue("vendor",     out var vendor);
        mushroom.TryGetValue("finished",   out var finished);
        mushroom.TryGetValue("successful", out var successful);

        var errors = new List<string>();
        
        if (parent == null && parentType != null)                 errors.Add("You must also specify the `ParentType` parameter when using the `Parent` parameter");
        if (parent != null && parentType == null)                 errors.Add("You must also specify the `Parent` parameter when using the `ParentType` parameter");
        if (children == null && childType != null)                errors.Add("You must also specify the `Children` parameter when using the `ChildType` parameter");
        if (children != null && childType == null)                errors.Add("You must also specify the `ChildType` parameter when using the `Children` parameter");
        if ((purchased is null or false) && (vendor is not null)) errors.Add("You must also specify the `Vendor` parameter when using the `Purchased` parameter");
        if ((purchased is not null) && (vendor is null))          errors.Add("You must also specify the `Purchased` parameter when using the `Vendor` parameter");
        if (finished is null && successful is not null)           errors.Add("You must also specify the `Successful` parameter when using the `Finished` parameter");

        if (errors.Any())
        {
            context.Result = new BadRequestObjectResult(errors);
            return;
        }
        
        await next();
    }
}