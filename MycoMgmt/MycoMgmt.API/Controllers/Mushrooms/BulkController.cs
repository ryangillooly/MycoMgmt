using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Repositories;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.API.Helpers;

namespace MycoMgmt.API.Controllers;

[Route("bulk")]
[ApiController]
public class BulkController : Controller
{
    private readonly IBulkRepository _bulkRepository;

    public BulkController(IBulkRepository repo)
    {
        _bulkRepository = repo;
    }

    [HttpPost]
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
        bool?   successful,
        bool    finished,
        string  createdOn,
        string  createdBy,
        int?    count = 1
    )
    {
        var bulk = new Bulk()
        {
            Name      = name,
            Strain    = strain,
            Finished  = finished,
            CreatedOn = DateTime.Parse(createdOn),
            CreatedBy = createdBy
        };

        if (recipe != null)
            bulk.Recipe = recipe;

        if (location != null)
            bulk.Location = location;

        if (notes != null)
            bulk.Notes = notes;

        if((parent == null && parentType != null ) || (parent != null && parentType == null))
            throw new ValidationException("If the Parent parameter has been provided, then the ParentType must also be provided");
        
        if((child == null && childType != null ) || (child != null && childType == null))
            throw new ValidationException("If the Child parameter has been provided, then the ChildType must also be provided");
        
        if (parent != null && parentType != null)
        {
            bulk.Parent     = parent;
            bulk.ParentType = parentType;
        }
        
        if (child != null && childType != null)
        {
            bulk.Child     = child;
            bulk.ChildType = childType;
        }

        if (successful != null)
            bulk.Successful = successful.Value;

        bulk.Tags.Add(bulk.IsSuccessful());

        var resultList = new List<string>();
        var bulkName = bulk.Name;
        
        if (count == 1)
        {
            resultList.Add(await _bulkRepository.Create(bulk));   
        }
        else
        {
            for (var i = 1; i <= count; i++)
            {
                bulk.Name = bulkName + "-" + i.ToString("D2");
                resultList.Add(await _bulkRepository.Create(bulk));
            }
        }

        return Created("", string.Join(",", resultList));
    }

    [HttpPut("{elementId}")]
    public async Task<IActionResult> Update
    (
        string elementId,
        string? name,
        string? strain,
        string? recipe,
        string? notes,
        string? location,
        string? parent,
        string? parentType,
        string? child,
        string? childType,
        bool? successful,
        bool? finished,
        string modifiedOn,
        string modifiedBy
    )
    {
        var bulk = new Bulk
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

        if (name != null)
            bulk.Name = name;

        if (recipe != null)
            bulk.Recipe = recipe;

        if (strain != null)
            bulk.Strain = strain;
        
        if (notes != null)
            bulk.Notes = notes;

        if (location != null)
            bulk.Location = location;

        if (parent != null && parentType != null)
        {
            bulk.Parent     = parent;
            bulk.ParentType = parentType;
        }
        
        if (child != null && childType != null)
        {
            bulk.Child     = child;
            bulk.ChildType = childType;
        }

        if (successful != null)
            bulk.Successful = successful.Value;

        if (finished != null)
            bulk.Finished = finished;

        var result = await _bulkRepository.Update(elementId, bulk);

        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var records = await _bulkRepository.GetAll();
        return Ok(records);
    }

    [HttpGet("id/{elementId}")]
    public async Task<IActionResult> GetById(string elementId)
    {
        var results = await _bulkRepository.GetById(elementId);
        return Ok(results);
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        var results = await _bulkRepository.GetByName(name);
        return Ok(results);
    }


    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name)
    {
        var results = await _bulkRepository.SearchByName(name);
        return Ok(results);
    }

    [HttpDelete("{elementId}")]
    public async Task<IActionResult> Delete(string elementId)
    {
        await _bulkRepository.Delete(elementId);
        return NoContent();
    }
}