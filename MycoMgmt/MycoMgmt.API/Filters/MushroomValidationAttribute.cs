using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MycoMgmt.Domain.Models.Mushrooms;

namespace MycoMgmt.API.Filters;

public class MushroomValidationAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var mushroom = context.ActionArguments["mushroom"] as Mushroom;

        if ((mushroom.Parent == null && mushroom.ParentType != null) ||
            (mushroom.Parent != null && mushroom.ParentType == null))
            new BadRequestObjectResult(
                "If the Parent parameter has been provided, then the ParentType must also be provided");

        if ((mushroom.Children == null && mushroom.ChildType != null) ||
            (mushroom.Children != null && mushroom.ChildType == null))
            new BadRequestObjectResult(
                "If the Children parameter has been provided, then the ChildType must also be provided");

        if ((mushroom.Purchased is null or false) && (mushroom.Vendor is not null))
            new BadRequestObjectResult("You cannot supply a Vendor if the item was not Purchased.");

        if (mushroom.Finished == null && mushroom.Successful != null)
            new BadRequestObjectResult(
                "When providing the Successful parameter, you must also specify the Finished parameter");

        await next();
    }
}