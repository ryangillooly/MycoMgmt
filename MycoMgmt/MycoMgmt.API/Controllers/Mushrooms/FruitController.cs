using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Repositories;
using MycoMgmt.Domain.Models.Mushrooms;

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
        string  name,
        string  strain,
        string? notes,
        decimal? wetWeight,
        decimal? dryWeight,
        string? location,
        string? parent,
        string? parentType,
        string? child,
        string? childType,
        bool?   successful,
        bool    finished,
        string  createdOn,
        string  createdBy,
        string? modifiedOn,
        string? modifiedBy,
        int?    count = 1
    )
    {
        var fruit = new Fruit()
        {
            Name      = name,
            Strain    = strain,
            Finished  = finished,
            CreatedOn = DateTime.Parse(createdOn),
            CreatedBy = createdBy
        };

        if (wetWeight != null)
            fruit.WetWeight = wetWeight.Value;
        
        if (dryWeight != null)
            fruit.DryWeight = dryWeight.Value;
        
        if (location != null)
            fruit.Location = location;

        if (notes != null)
            fruit.Notes = notes;

        if((parent == null && parentType != null ) || (parent != null && parentType == null))
            throw new ValidationException("If the Parent parameter has been provided, then the ParentType must also be provided");
        
        if((child == null && childType != null ) || (child != null && childType == null))
            throw new ValidationException("If the Child parameter has been provided, then the ChildType must also be provided");
        
        if (parent != null && parentType != null)
        {
            fruit.Parent     = parent;
            fruit.ParentType = parentType;
        }
        
        if (child != null && childType != null)
        {
            fruit.Child     = child;
            fruit.ChildType = childType;
        }

        if (successful != null)
            fruit.Successful = successful.Value;

        if (modifiedOn != null)
            fruit.ModifiedOn = DateTime.Parse(modifiedOn);

        if (modifiedBy != null)
            fruit.ModifiedBy = modifiedBy;

        fruit.Tags.Add(fruit.IsSuccessful());

        var resultList = new List<string>();
        var bulkName = fruit.Name;

        if (count == 1)
        {
            resultList.Add(await _fruitRepository.Create(fruit));   
        }
        else
        {
            for (var i = 1; i <= count; i++)
            {
                fruit.Name = bulkName + "-" + i.ToString("D2");
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
        string   modifiedOn,
        string   modifiedBy
    )
    {
        var fruit = new Fruit()
        {
            ModifiedOn = DateTime.Parse(modifiedOn),
            ModifiedBy = modifiedBy
        };
        
        if((parent == null && parentType != null ) || (parent != null && parentType == null))
            throw new ValidationException("If the Parent parameter has been provided, then the ParentType must also be provided");
        
        if((child == null && childType != null ) || (child != null && childType == null))
            throw new ValidationException("If the Child parameter has been provided, then the ChildType must also be provided");
        
        if (finished == null && successful != null)
            throw new ValidationException("When providing the Successful parameter, you must also specify the Finished parameter");

        if (wetWeight != null)
            fruit.WetWeight = wetWeight.Value;
        
        if (dryWeight != null)
            fruit.DryWeight = dryWeight.Value;
        
        if (name != null)
            fruit.Name = name;

        if (strain != null)
            fruit.Strain = strain;
        
        if (notes != null)
            fruit.Notes = notes;

        if (location != null)
            fruit.Location = location;

        if (parent != null && parentType != null)
        {
            fruit.Parent     = parent;
            fruit.ParentType = parentType;
        }
        
        if (child != null && childType != null)
        {
            fruit.Child     = child;
            fruit.ChildType = childType;
        }

        if (successful != null)
            fruit.Successful = successful.Value;

        if (finished != null)
            fruit.Finished = finished;

        var result = await _fruitRepository.Update(elementId, fruit);

        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var records = await _fruitRepository.GetAll();
        return Ok(records);
    }

    [HttpGet("id/{elementId}")]
    public async Task<IActionResult> GetById(string elementId)
    {
        var results = await _fruitRepository.GetById(elementId);
        return Ok(results);
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        var results = await _fruitRepository.GetByName(name);
        return Ok(results);
    }


    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name)
    {
        var results = await _fruitRepository.SearchByName(name);
        return Ok(results);
    }

    [HttpDelete("{elementId}")]
    public async Task<IActionResult> Delete(string elementId)
    {
        await _fruitRepository.Delete(elementId);
        return NoContent();
    }
}