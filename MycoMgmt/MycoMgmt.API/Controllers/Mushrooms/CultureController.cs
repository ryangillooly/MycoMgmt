using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Filters;
using MycoMgmt.API.Models;
using MycoMgmt.Core.Helpers;
using MycoMgmt.Infrastructure.Repositories;
using MycoMgmt.Domain.Models.Mushrooms;
using static MycoMgmt.Infrastructure.Helpers.IActionRepositoryExtensions;

namespace MycoMgmt.API.Controllers;

[Route("culture")]
[ApiController]
public class CultureController : BaseController<CultureController>
{
    [HttpPost]
    [MushroomValidation]
    public async Task<IActionResult> Create (CultureControllerViewModel values)
    {
        var culture = new Culture
        {
            Name         = values.name,
            Type         = values.type,
            Recipe       = values.recipe,
            Location     = values.location,
            Notes        = values.notes,
            Strain       = values.strain,
            Parent       = values.parent,
            ParentType   = values.parentType,
            Children     = values.child,
            ChildType    = values.childType,
            Vendor       = values.vendor,
            Purchased    = values.purchased, 
            Finished     = values.finished,
            Successful   = values.successful,
            FinishedOn   = values.finishedOn is null ? null : DateTime.Parse(values.finishedOn),
            InoculatedOn = values.inoculatedOn is null ? null : DateTime.Parse(values.inoculatedOn),
            InoculatedBy = values.inoculatedBy,
            CreatedOn    = DateTime.Parse(values.createdOn),
            CreatedBy    = values.createdBy
        };

        culture.Tags.Add(culture.IsSuccessful());
        culture.Status  = culture.IsSuccessful();
        var result  = await Repository.CreateEntities(Logger, culture, values.count);
        return Created("", result);
    }
    
    [HttpPut("{Id}")]
    public async Task<IActionResult> Update
    (
        string  Id,
        string? name,
        string? type,
        string? strain,
        string? recipe,
        string? notes,
        string? location,
        string? parent,
        string? parentType,
        string? child,
        string? childType,
        string? vendor,
        bool?   successful,
        bool?   finished,
        string? finishedOn,
        string? inoculatedOn,
        string? inoculatedBy,
        string  modifiedOn,
        string  modifiedBy
    )
    {
        var culture = new Culture
        {
            Id    = Id,
            Name         = name,
            Strain       = strain,
            Type         = type,
            Notes        = notes,
            Recipe       = recipe,
            Location     = location,
            Vendor       = vendor,
            Successful   = successful,
            Parent       = parent,
            ParentType   = parentType,
            Children     = child,
            ChildType    = childType,
            InoculatedBy = inoculatedBy,
            Finished     = finished,
            FinishedOn   = finishedOn is null ? null : DateTime.Parse(finishedOn),
            InoculatedOn = inoculatedOn is null ? null : DateTime.Parse(inoculatedOn),
            ModifiedOn   = DateTime.Parse(modifiedOn),
            ModifiedBy   = modifiedBy
        };
        
        culture.Validate();
        return Ok(await Repository.Update(culture));
    }
    
    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete(string Id)
    {
        await Repository.Delete(new Culture { Id = Id });
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(int skip = 0, int limit = 20) => Ok(await Repository.GetAll(new Culture(), skip, limit));

    [HttpGet("id/{Id}")]
    public async Task<IActionResult> GetById(string Id) => Ok(await Repository.GetById(new Culture { Id = Id }));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name) => Ok(await Repository.GetByName(new Culture { Name = name }));

    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name) => Ok(await Repository.SearchByName(new Culture { Name = name }));
}