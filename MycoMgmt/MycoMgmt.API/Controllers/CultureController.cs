using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Repositories;
using MycoMgmt.Domain.Models;
using MycoMgmt.Domain.Models.Mushrooms;

namespace MycoMgmt.API.Controllers;

[Route("cultures")]
[ApiController]
public class CultureController : Controller
{
    private readonly ICultureRepository _cultureRepository;

    public CultureController(ICultureRepository repo)
    {
        _cultureRepository = repo;
    }

    [HttpPost]
    public async Task<IActionResult> Create
    (
        string name,
        string type,
        string strain,
        string? recipe,
        string? location,
        string? parent,
        string? child,
        string? vendor,
        bool? successful,
        bool finished,
        string createdOn,
        string createdBy,
        string? modifiedOn,
        string? modifiedBy
    )
    {
        var culture = new Culture()
        {
            Name = name,
            Type = type.Replace(" ",""),
            Strain = strain,
            Finished = finished,
            CreatedOn = DateTime.Parse(createdOn),
            CreatedBy = createdBy
        };

        if (recipe != null)
            culture.Recipe = recipe;

        if (location != null)
            culture.Location = location;

        if (parent != null)
            culture.Parent = parent;

        if (child != null)
            culture.Child = child;

        if (vendor != null)
            culture.Vendor = vendor;

        if (successful != null)
            culture.Successful = successful.Value;

        if (modifiedOn != null)
            culture.ModifiedOn = DateTime.Parse(modifiedOn);

        if (modifiedBy != null)
            culture.ModifiedBy = modifiedBy;
        
        culture.Tags.Add(culture.IsSuccessful());
        culture.Tags.Add(culture.Type);

        var result = await _cultureRepository.Create(culture);

        return Created("", result);
    }

    [HttpPut("{elementId}")]
    public async Task<IActionResult> Update
    (
        string elementId,
        string? name,
        string? type,
        string? strain,
        string? recipe,
        string? location,
        string? parent,
        string? child,
        string? vendor,
        bool? successful,
        bool? finished,
        string modifiedOn,
        string modifiedBy
    )
    {
        var culture = new Culture
        {
            ModifiedOn = DateTime.Parse(modifiedOn),
            ModifiedBy = modifiedBy
        };
        
        if (name != null)
            culture.Name = name;
        
        if (strain != null)
            culture.Strain = strain;
        
        if (type != null)
            culture.Type = type;
        
        if (recipe != null)
            culture.Recipe = recipe;

        if (location != null)
            culture.Location = location;

        if (parent != null)
            culture.Parent = parent;

        if (child != null)
            culture.Child = child;

        if (vendor != null)
            culture.Vendor = vendor;

        if (successful != null)
            culture.Successful = successful.Value;

        if (finished != null)
            culture.Finished = finished;

        var result = await _cultureRepository.Update(elementId, culture);

        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var records = await _cultureRepository.GetAll();
        return Ok(records);
    }

    [HttpGet("id/{elementId}")]
    public async Task<IActionResult> GetById(string elementId)
    {
        var results = await _cultureRepository.GetById(elementId);
        return Ok(results);
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        var results = await _cultureRepository.GetByName(name);
        return Ok(results);
    }


    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name)
    {
        var results = await _cultureRepository.SearchByName(name);
        return Ok(results);
    }

    [HttpDelete("{elementId}")]
    public async Task<IActionResult> Delete(string elementId)
    {
        await _cultureRepository.Delete(elementId);
        return NoContent();
    }
}