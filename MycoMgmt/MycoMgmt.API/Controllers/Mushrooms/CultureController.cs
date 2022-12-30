using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Repositories;
using MycoMgmt.API.Helpers;
using MycoMgmt.Domain.Models.Mushrooms;
using Neo4j.Driver;
using Newtonsoft.Json;

namespace MycoMgmt.API.Controllers;

[Route("culture")]
[ApiController]
public class CultureController : Controller
{
    private readonly ICultureRepository _cultureRepository;
    private readonly ILogger<CultureController> _logger;

    public CultureController(ICultureRepository repo, ILogger<CultureController> logger)
    {
        _cultureRepository = repo;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create
    (
        string  name,
        string  type,
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
        bool?   purchased,
        string? vendor,
        string? finishedOn,
        string? inoculatedOn,
        string? inoculatedBy,
        string  createdOn,
        string  createdBy,
        int?    count = 1
    )
    {
        if((parent is null && parentType is not null ) || (parent is not null && parentType is null))
            throw new ValidationException("If the Parent parameter has been provided, then the ParentType must also be provided");
        
        if((child is null && childType is not null ) || (child is not null && childType is null))
            throw new ValidationException("If the Children parameter has been provided, then the ChildType must also be provided");
        
        if(purchased is null or false && vendor is not null )
            throw new ValidationException("You cannot supply a Vendor if the item was not Purchased.");
        
        var culture = new Culture
        {
            Name         = name,
            Type         = type,
            Recipe       = recipe,
            Location     = location,
            Notes        = notes,
            Strain       = strain,
            Parent       = parent,
            ParentType   = parentType,
            Children     = child,
            ChildType    = childType,
            Vendor       = vendor,
            Purchased    = purchased, 
            Finished     = finished,
            Successful   = successful,
            FinishedOn   = finishedOn is null ? null : DateTime.Parse(finishedOn),
            InoculatedOn = inoculatedOn is null ? null : DateTime.Parse(inoculatedOn),
            InoculatedBy = inoculatedBy,
            CreatedOn    = DateTime.Parse(createdOn),
            CreatedBy    = createdBy
        };

        culture.Status = culture.IsSuccessful();

        var resultList = new List<IEntity>();
        var cultureName = culture.Name;
        
        if (count == 1)
        {
            var results = await _cultureRepository.Create(culture);
            resultList = resultList.Concat(results).ToList();
        }
        else
        {
            for (var i = 1; i <= count; i++)
            {
                culture.Name = cultureName + "-" + i.ToString("D2");
                var results = await _cultureRepository.Create(culture);
                resultList = resultList.Concat(results).ToList();
            }
        }

        var nodeList = resultList
                                            .Where(entity => entity is INode)
                                            .Select(item => new 
                                           {
                                               Name      = item.Properties.TryGetValue("Name", out var name) ? (string?) name : null,
                                               ElementId = (string? )item.ElementId
                                           })
                                           .ToList();
        
        var result = JsonConvert.SerializeObject(nodeList);
        
         _logger.LogInformation("New Cultures Created - {cultureName}", nodeList.Select(item => $"{item.Name} ({item.ElementId})"));
         
        return Created("", result);
    }

    [HttpPut("{elementId}")]
    public async Task<IActionResult> Update
    (
        string  elementId,
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
        if((parent == null && parentType != null ) || (parent != null && parentType == null))
            throw new ValidationException("If the Parent parameter has been provided, then the ParentType must also be provided");
        
        if((child == null && childType != null ) || (child != null && childType == null))
            throw new ValidationException("If the Children parameter has been provided, then the ChildType must also be provided");
        
        if (finished == null && successful != null)
            throw new ValidationException("When providing the Successful parameter, you must also specify the Finished parameter");

        var culture = new Culture
        {
            ElementId    = elementId,
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
        
        return Ok(await _cultureRepository.Update(culture));
    }
    
    [HttpDelete("{elementId}")]
    public async Task<IActionResult> Delete(string elementId)
    {
        await _cultureRepository.Delete(new Culture { ElementId = elementId });
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(int skip = 0, int limit = 20) => Ok(await _cultureRepository.GetAll(new Culture(), skip, limit));

    [HttpGet("id/{elementId}")]
    public async Task<IActionResult> GetById(string elementId) => Ok(await _cultureRepository.GetById(new Culture { ElementId = elementId }));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name) => Ok(await _cultureRepository.GetByName(new Culture { Name = name }));

    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name) => Ok(await _cultureRepository.SearchByName(new Culture { Name = name }));
}