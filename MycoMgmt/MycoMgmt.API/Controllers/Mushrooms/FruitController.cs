using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Filters;
using MycoMgmt.Core.Helpers;
using MycoMgmt.Infrastructure.Repositories;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.Infrastructure.Helpers;

namespace MycoMgmt.API.Controllers;

[Route("fruit")]
[ApiController]
public class FruitController : BaseController<FruitController>
{
    [HttpPost]
    [MushroomValidation]
    public async Task<IActionResult> Create
    (
        string   name,
        string   strain,
        string?  notes,
        decimal? wetWeight,
        decimal? dryWeight,
        string?  location,
        string?  parent,
        string?  parentType,
        string?  child,
        string?  childType,
        string?  vendor,
        bool?    successful,
        bool?    purchased,
        bool     finished,
        string?  harvestedOn,
        string?  harvestedBy,
        string   createdOn,
        string   createdBy,
        int?     count = 1
    )
    {
        var fruit = new Fruit()
        {
            Name         = name,
            WetWeight    = wetWeight,
            DryWeight    = dryWeight,
            Location     = location,
            Notes        = notes,
            Parent       = parent,
            ParentType   = parentType,
            Children     = child,
            ChildType    = childType,
            Vendor       = vendor,  
            Strain       = strain,
            Successful   = successful,
            Purchased    = purchased,
            Finished     = finished,
            HarvestedOn  = harvestedOn is null ? null : DateTime.Parse(harvestedOn),
            HarvestedBy  = harvestedBy,
            CreatedOn    = DateTime.Parse(createdOn),
            CreatedBy    = createdBy
        };
        
        fruit.Tags.Add(fruit.IsSuccessful());
        fruit.Status  = fruit.IsSuccessful();
        var result  = await Repository.CreateEntities(Logger, fruit, count);
        return Created("", string.Join(",", result));
    }

    [HttpPut("{id}")]
    [MushroomValidation]
    public async Task<IActionResult> Update
    (
        string   id,
        string?  name,
        decimal? wetWeight,
        decimal? dryWeight,
        string?  strain,
        string?  notes,
        string?  location,
        string?  parent,
        string?  parentType,
        string?  child,
        string?  childType,
        string?  vendor,
        bool?    successful,
        bool?    finished,
        bool?    purchased,
        string?  harvestedOn,
        string?  harvestedBy,
        string?  inoculatedOn,
        string?  inoculatedBy,
        string   modifiedOn,
        string   modifiedBy
    )
    {
        var fruit = new Fruit()
        {
            Id           = id,
            Name         = name,
            WetWeight    = wetWeight,
            DryWeight    = dryWeight,
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
            Purchased    = purchased, 
            HarvestedOn  = harvestedOn is null ? null : DateTime.Parse(harvestedOn),
            HarvestedBy  = harvestedBy,
            InoculatedOn = inoculatedOn is null ? null : DateTime.Parse(inoculatedOn),
            InoculatedBy = inoculatedBy,
            ModifiedOn   = DateTime.Parse(modifiedOn),
            ModifiedBy   = modifiedBy
        };
        
        return Ok(await Repository.Update(fruit));
    }
    
    
    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete(string Id)
    {
        await Repository.Delete(new Fruit { Id = Id });
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await Repository.GetAll(new Fruit(), skip, limit));

    [HttpGet("id/{Id}")]
    public async Task<IActionResult> GetById(string Id) => Ok(await Repository.GetById(new Fruit { Id = Id }));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name) => Ok(await Repository.GetByName(new Fruit { Name = name }));

    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name) => Ok(await Repository.SearchByName(new Fruit { Name = name }));
}