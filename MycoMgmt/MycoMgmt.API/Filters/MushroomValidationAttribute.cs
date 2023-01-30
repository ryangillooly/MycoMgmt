using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MycoMgmt.Core.Contracts.Mushroom;
using MycoMgmt.Core.Models.Mushrooms;

namespace MycoMgmt.API.Filters;

public class MushroomValidationAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpMethod = context.HttpContext.Request.Method;
        var errors           = new Dictionary<string, string>();
        /*var mushroom = new Mushroom
        {
            Name = request.Name,
            Type = request.Type,
            Notes = request.Notes,
            CreatedBy = request.CreatedBy,
            CreatedOn = request.CreatedOn,
            ModifiedBy = request.ModifiedBy,
            ModifiedOn = request.ModifiedOn,
            Strain = request.Strain,
            Location = request.Location,
            Parent = request.Parent,
            ParentType = request.ParentType,
            Children = request.Children,
            ChildType = request.ChildType,
            Successful = request.Successful,
            Finished = request.Finished,
            FinishedOn = request.FinishedOn,
            InoculatedOn = request.InoculatedOn,
            InoculatedBy = request.InoculatedBy,
            Recipe = request.Recipe,
            Purchased = request.Purchased,
            Vendor = request.Vendor
        };
        */
        
        var action     = context.RouteData.Values["action"];
        var controller = context.RouteData.Values["controller"];
        var mushroom = new Mushroom();
        
        switch (httpMethod)
        {
            case "POST":
            {
                var request  = (CreateMushroomRequest) context.ActionArguments["request"]!;
                var config   = new MapperConfiguration(cfg => cfg.CreateMap<CreateMushroomRequest, Mushroom>()); 
                var mapper   = new Mapper(config);
                mushroom = mapper.Map<Mushroom>(request);
                
                if (string.IsNullOrEmpty(mushroom.Name))   errors.Add(nameof(mushroom.Name),   SingleFieldMessage(nameof(mushroom.Name)));
                if (string.IsNullOrEmpty(mushroom.Strain)) errors.Add(nameof(mushroom.Strain), SingleFieldMessage(nameof(mushroom.Strain)));
                //if (mushroom.CreatedOn is null) errors.Add(nameof(mushroom.CreatedOn), SingleFieldMessage(nameof(mushroom.CreatedOn)));
                if (mushroom.CreatedBy is null) errors.Add(nameof(mushroom.CreatedBy), SingleFieldMessage(nameof(mushroom.CreatedBy)));

                var list = new List<string>{"Culture", "Spawn", "Recipe"};

                if (list.Contains(controller))
                    if (string.IsNullOrEmpty(mushroom.Type)) errors.Add(nameof(mushroom.Type), SingleFieldMessage(nameof(mushroom.Type)));

                if (controller is not "Fruit")
                {
                    if (mushroom.InoculatedOn is null) errors.Add(nameof(mushroom.InoculatedOn), SingleFieldMessage(nameof(mushroom.InoculatedOn)));
                    if (mushroom.InoculatedBy is null) errors.Add(nameof(mushroom.InoculatedBy), SingleFieldMessage(nameof(mushroom.InoculatedBy)));
                }
                
                /*
                if (controller is "Fruit" && mushroom.Finished is true)
                {
                    if (mushroom.HarvestedOn is null) errors.Add(nameof(mushroom.HarvestedOn), SingleFieldMessage(nameof(mushroom.HarvestedOn)));
                    if (mushroom.HarvestedBy is null) errors.Add(nameof(mushroom.HarvestedBy), SingleFieldMessage(nameof(mushroom.HarvestedBy)));
                }
                */

                break;
            }
            case "PUT":
            {
                var request  = (UpdateMushroomRequest) context.ActionArguments["request"]!;
                var config   = new MapperConfiguration(cfg => cfg.CreateMap<UpdateMushroomRequest, Mushroom>()); 
                var mapper   = new Mapper(config);
                mushroom = mapper.Map<Mushroom>(request);
                
                var type = mushroom.GetType();
                var fields = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                var fieldsProvided = new List<string>();
                 
                foreach (var field in fields)
                {
                    if (field.Name is "ModifiedOn" or "ModifiedBy") continue;
                    
                    var value = field.GetValue(mushroom);
                    var a = value?.GetType();
                    
                    if (value?.GetType().Name == "String")
                    {
                        if (!string.IsNullOrWhiteSpace(value.ToString()))
                        {
                            fieldsProvided.Add(field.Name);
                        }
                    }
                    else
                    {
                        if (value is not null)
                            fieldsProvided.Add(field.Name);
                    }
                }

                var fieldList = fieldsProvided.Where(name => name is not "ModifiedOn" and not "ModifiedBy" and not "Id" and not "Tags" and not "EntityType");
                
                if(fieldList.Any())
                {
                    //if (mushroom.ModifiedOn is null) errors.Add(nameof(mushroom.ModifiedOn), SingleFieldMessage(nameof(mushroom.ModifiedOn)));
                    if (mushroom.ModifiedBy is null) errors.Add(nameof(mushroom.ModifiedBy), SingleFieldMessage(nameof(mushroom.ModifiedBy)));
                }
                else
                {
                    errors.Add("Validation", $"The minimum required fields are ModifiedBy, and at least 1 other field");
                }
                
                /*
                if (controller is "Fruit" && mushroom.Finished is true)
                {
                    if (mushroom.HarvestedOn is null) errors.Add(nameof(mushroom.HarvestedOn), SingleFieldMessage(nameof(mushroom.HarvestedOn)));
                    if (mushroom.HarvestedBy is null) errors.Add(nameof(mushroom.HarvestedBy), SingleFieldMessage(nameof(mushroom.HarvestedBy)));
                }
                */
                
                break;
            }
        }

        if (mushroom.Parent     is null          && mushroom.ParentType is not null) errors.Add(nameof(mushroom.Parent)    , DualFieldMessage(nameof(mushroom.Parent),     nameof(mushroom.ParentType)));
        if (mushroom.ParentType is null          && mushroom.Parent     is not null) errors.Add(nameof(mushroom.ParentType), DualFieldMessage(nameof(mushroom.ParentType), nameof(mushroom.Parent)));
        if (mushroom.Children   is null          && mushroom.ChildType  is not null) errors.Add(nameof(mushroom.Children)  , DualFieldMessage(nameof(mushroom.Children),   nameof(mushroom.ChildType)));
        if (mushroom.ChildType  is null          && mushroom.Children   is not null) errors.Add(nameof(mushroom.ChildType) , DualFieldMessage(nameof(mushroom.ChildType),  nameof(mushroom.Children)));
        if (mushroom.Purchased  is null or false && mushroom.Vendor     is not null) errors.Add(nameof(mushroom.Purchased) , DualFieldMessage(nameof(mushroom.Purchased),  nameof(mushroom.Vendor)));
        if (mushroom.Vendor     is null          && mushroom.Purchased  is not null) errors.Add(nameof(mushroom.Vendor)    , DualFieldMessage(nameof(mushroom.Vendor),     nameof(mushroom.Purchased)));
        if (mushroom.Finished   is null          && mushroom.Successful is not null) errors.Add(nameof(mushroom.Finished  ), DualFieldMessage(nameof(mushroom.Finished),   nameof(mushroom.Successful)));

        if (errors.Any())
        {
            var errorResponse = new
            {
                type    = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                title   = "One or more validation errors occurred.",
                status  = HttpStatusCode.BadRequest,
                traceId = context.HttpContext.TraceIdentifier,
                errors
            };

            context.Result = new BadRequestObjectResult(errorResponse);
            return;
        }

        await next();
    }

    private static string SingleFieldMessage(string field) => $"The `{field}` field is required";
    private static string DualFieldMessage(string nullField, string nonNullField) => $"You must specify the `{nullField}` field when using the `{nonNullField}` field";
}