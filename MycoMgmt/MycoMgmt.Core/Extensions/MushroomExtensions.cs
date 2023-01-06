using System.ComponentModel.DataAnnotations;
using MycoMgmt.Domain.Models.Mushrooms;

namespace MycoMgmt.Core.Helpers;

public static class MushroomExtensions
{
    public static Mushroom ValidateParent(this Mushroom mushroom) =>
        ((mushroom.Parent == null && mushroom.ParentType != null) || (mushroom.Parent != null && mushroom.ParentType == null))
        ? throw new ValidationException("If the Parent parameter has been provided, then the ParentType must also be provided")
        : mushroom;

    public static Mushroom ValidateChild(this Mushroom mushroom) =>
        ((mushroom.Children == null && mushroom.ChildType != null) || (mushroom.Children != null && mushroom.ChildType == null))
        ? throw new ValidationException("If the Children parameter has been provided, then the ChildType must also be provided")
        : mushroom;

    public static Mushroom ValidateVendor(this Mushroom mushroom) =>
        ((mushroom.Purchased is null or false) && (mushroom.Vendor is not null))
        ? throw new ValidationException("You cannot supply a Vendor if the item was not Purchased.")
        : mushroom;
    
    public static Mushroom ValidateSuccess(this Mushroom mushroom) =>
        (mushroom.Finished == null && mushroom.Successful != null)
        ? throw new ValidationException("When providing the Successful parameter, you must also specify the Finished parameter")
        : mushroom;

    public static void Validate(this Mushroom mushroom) =>
        mushroom
            .ValidateChild()
            .ValidateParent()
            .ValidateSuccess()
            .ValidateVendor();
}