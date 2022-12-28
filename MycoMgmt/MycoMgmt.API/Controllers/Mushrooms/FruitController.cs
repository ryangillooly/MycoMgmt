using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Repositories;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.API.Helpers;

namespace MycoMgmt.API.Controllers;

[Route("fruit")]
[ApiController]
public class FruitController : Controller
{
    private readonly IFruitRepository _fruitRepository;

    public FruitController(IFruitRepository repo)
    {
        _fruitRepository = repo;
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
        bool?    successful,
        bool     finished,
        string?  finishedOn,
        string?  inoculatedOn,
        string?  inoculatedBy,
        string   createdOn,
        string   createdBy,
        int?     count = 1
    )
    {
        if((parent == null && parentType != null ) || (parent != null && parentType == null))
            throw new ValidationException("If the Parent parameter has been provided, then the ParentType must also be provided");
        
        if((child == null && childType != null ) || (child != null && childType == null))
            throw new ValidationException("If the Child parameter has been provided, then the ChildType must also be provided");

        var fruit = new Fruit()
        {
            Name         = name,
            WetWeight    = wetWeight,
            DryWeight    = dryWeight,
            Location     = location,
            Notes        = notes,
            Parent       = parent,
            ParentType   = parentType,
            Child        = child,
            ChildType    = childType,
            Strain       = strain,
            Successful   = successful,
            Finished     = finished,
            FinishedOn   = finishedOn is null ? null : DateTime.Parse(finishedOn),
            InoculatedOn = inoculatedOn is null ? null : DateTime.Parse(inoculatedOn),
            InoculatedBy = inoculatedBy,
            CreatedOn    = DateTime.Parse(createdOn),
            CreatedBy    = createdBy
        };
        
        fruit.Tags.Add(fruit.IsSuccessful());

        var resultList = new List<string>();
        var fruitName = fruit.Name;

        if (count == 1)
        {
            resultList.Add(await _fruitRepository.Create(fruit));   
        }
        else
        {
            for (var i = 1; i <= count; i++)
            {
                fruit.Name = fruitName + "-" + i.ToString("D2");
                resultList.Add(await _fruitRepository.Create(fruit));
            }
        }

        return Created("", string.Join(",", resultList));
    }

    [HttpPut("{elementId}")]
    public async Task<IActionResult> Update
    (
        string   elementId,
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
        bool?    successful,
        bool?    finished,
        string?  finishedOn,
        string?  inoculatedOn,
        string?  inoculatedBy,
        string   modifiedOn,
        string   modifiedBy
    )
    {
        if((parent == null && parentType != null ) || (parent != null && parentType == null))
            throw new ValidationException("If the Parent parameter has been provided, then the ParentType must also be provided");
        
        if((child == null && childType != null ) || (child != null && childType == null))
            throw new ValidationException("If the Child parameter has been provided, then the ChildType must also be provided");
        
        if (finished == null && successful != null)
            throw new ValidationException("When providing the Successful parameter, you must also specify the Finished parameter");

        var fruit = new Fruit()
        {
            ElementId    = elementId,
            Name         = name,
            WetWeight    = wetWeight,
            DryWeight    = dryWeight,
            Strain       = strain,
            Notes        = notes,
            Location     = location,
            Parent       = parent,
            ParentType   = parentType,
            Child        = child,
            ChildType    = childType,
            Successful   = successful,
            Finished     = finished,
            FinishedOn   = finishedOn is null ? null : DateTime.Parse(finishedOn),
            InoculatedOn = inoculatedOn is null ? null : DateTime.Parse(inoculatedOn),
            InoculatedBy = inoculatedBy,
            ModifiedOn   = DateTime.Parse(modifiedOn),
            ModifiedBy   = modifiedBy
        };
        
        return Ok(await _fruitRepository.Update(fruit));
    }
    
    
    [HttpDelete("{elementId}")]
    public async Task<IActionResult> Delete(string elementId)
    {
        await _fruitRepository.Delete(new Fruit { ElementId = elementId });
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _fruitRepository.GetAll(new Fruit()));

    [HttpGet("id/{elementId}")]
    public async Task<IActionResult> GetById(string elementId) => Ok(await _fruitRepository.GetById(new Fruit { ElementId = elementId }));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name) => Ok(await _fruitRepository.GetByName(new Fruit { Name = name }));

    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name) => Ok(await _fruitRepository.SearchByName(new Fruit { Name = name }));
}