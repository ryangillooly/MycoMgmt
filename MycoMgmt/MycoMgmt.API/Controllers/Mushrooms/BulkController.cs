using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Filters;
using MycoMgmt.Core.Helpers;
using MycoMgmt.Infrastructure.Repositories;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.Infrastructure.Helpers;

namespace MycoMgmt.API.Controllers;

[Route("bulk")]
[ApiController]
public class BulkController : BaseController<BulkController>
{
    [HttpPost]
    [MushroomValidation]
    public async Task<IActionResult> Create
    (
        string  name,
        string  strain,
        string? recipe,
        string? notes,
        string? location,
        string? parent,
        string? parentType,
        string? child,
        string? childType,
        string? vendor,
        bool?   successful,
        bool    finished,
        string? finishedOn,
        string? inoculatedOn,
        string? inoculatedBy,
        string  createdOn,
        string  createdBy,
        int?    count = 1
    )
    {
        var bulk = new Bulk()
        {
            Name         = name,
            Strain       = strain,
            Recipe       = recipe,
            Location     = location,
            Notes        = notes,
            Parent       = parent,
            ParentType   = parentType,
            Children     = child,
            ChildType    = childType,
            Vendor       = vendor,
            Successful   = successful,
            Finished     = finished,
            FinishedOn   = finishedOn is null ? null : DateTime.Parse(finishedOn),
            InoculatedOn = inoculatedOn is null ? null : DateTime.Parse(inoculatedOn),
            InoculatedBy = inoculatedBy,
            CreatedOn    = DateTime.Parse(createdOn),
            CreatedBy    = createdBy
        };
        
        bulk.Tags.Add(bulk.IsSuccessful());
        bulk.Status = bulk.IsSuccessful();
        var result  = await Repository.CreateEntities(Logger, bulk, count);
        return Created("", result);
    }

    [HttpPut("{id}")]
    [MushroomValidation]
    public async Task<IActionResult> Update
    (
        string  id,
        string? name,
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
        var bulk = new Bulk
        {
            Id           = id,
            Name         = name,
            Recipe       = recipe,
            Strain       = strain,
            Notes        = notes,
            Location     = location,
            Parent       = parent,
            ParentType   = parentType,
            Children     = child,
            ChildType    = childType,
            Vendor       = vendor, 
            Successful   = successful,
            Finished     = finished,
            FinishedOn   = finishedOn is null ? null : DateTime.Parse(finishedOn),
            InoculatedOn = inoculatedOn is null ? null : DateTime.Parse(inoculatedOn),
            InoculatedBy = inoculatedBy,
            ModifiedOn   = DateTime.Parse(modifiedOn),
            ModifiedBy   = modifiedBy
        };
        
        return Ok(await Repository.Update(bulk));
    }
    
    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await Repository.Delete(new Bulk { Id = id });
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await Repository.GetAll(new Bulk(), skip, limit));

    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetById(string id) => Ok(await Repository.GetById(new Bulk { Id = id }));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name) => Ok(await Repository.GetByName(new Bulk { Name = name }));

    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name) => Ok(await Repository.SearchByName(new Bulk { Name = name }));
}