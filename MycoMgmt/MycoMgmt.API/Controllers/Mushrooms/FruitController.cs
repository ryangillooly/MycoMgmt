using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Infrastructure.Repositories;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.Infrastructure.Helpers;

namespace MycoMgmt.API.Controllers;

[Route("fruit")]
[ApiController]
public class FruitController : Controller
{
    private readonly ActionRepository _fruitRepository;
    private readonly ILogger<FruitController> _logger;

    public FruitController(ActionRepository repo, ILogger<FruitController> logger)
    {
        _fruitRepository = repo;
        _logger = logger;
    }

    [HttpPost]
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
        if((parent == null && parentType != null ) || (parent != null && parentType == null))
            throw new ValidationException("If the Parent parameter has been provided, then the ParentType must also be provided");
        
        if((child == null && childType != null ) || (child != null && childType == null))
            throw new ValidationException("If the Children parameter has been provided, then the ChildType must also be provided");

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
        
        var result  = await _fruitRepository.CreateEntities(_logger, fruit, count);
        
        return Created("", string.Join(",", result));
    }

    [HttpPut("{Id}")]
    public async Task<IActionResult> Update
    (
        string   Id,
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
        if((parent == null && parentType != null ) || (parent != null && parentType == null))
            throw new ValidationException("If the Parent parameter has been provided, then the ParentType must also be provided");
        
        if((child == null && childType != null ) || (child != null && childType == null))
            throw new ValidationException("If the Children parameter has been provided, then the ChildType must also be provided");
        
        if (finished == null && successful != null)
            throw new ValidationException("When providing the Successful parameter, you must also specify the Finished parameter");

        var fruit = new Fruit()
        {
            Id    = Id,
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
        
        return Ok(await _fruitRepository.Update(fruit));
    }
    
    
    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete(string Id)
    {
        await _fruitRepository.Delete(new Fruit { Id = Id });
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await _fruitRepository.GetAll(new Fruit(), skip, limit));

    [HttpGet("id/{Id}")]
    public async Task<IActionResult> GetById(string Id) => Ok(await _fruitRepository.GetById(new Fruit { Id = Id }));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name) => Ok(await _fruitRepository.GetByName(new Fruit { Name = name }));

    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name) => Ok(await _fruitRepository.SearchByName(new Fruit { Name = name }));
}