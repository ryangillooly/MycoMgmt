using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Repositories;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.API.Helpers;

namespace MycoMgmt.API.Controllers;

[Route("spawn")]
[ApiController]
public class SpawnController : Controller
{
    private readonly ISpawnRepository _spawnRepository;

    public SpawnController(ISpawnRepository repo)
    {
        _spawnRepository = repo;
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
        string  createdOn,
        string  createdBy,
        int?    count = 1
    )
    {
        if((parent == null && parentType != null ) || (parent != null && parentType == null))
            throw new ValidationException("If the Parent parameter has been provided, then the ParentType must also be provided");
        
        if((child == null && childType != null ) || (child != null && childType == null))
            throw new ValidationException("If the Child parameter has been provided, then the ChildType must also be provided");

        var spawn = new Spawn()
        {
            Name       = name,
            Recipe     = recipe,
            Location   = location,
            Notes      = notes,
            Type       = type,
            Strain     = strain,
            Finished   = finished,
            Successful = successful,
            Parent     = parent,
            ParentType = parentType,
            Child      = child,
            ChildType  = childType,
            CreatedOn  = DateTime.Parse(createdOn),
            CreatedBy  = createdBy
        };
        
        spawn.Tags.Add(spawn.IsSuccessful());
        spawn.Tags.Add(spawn.Type);
        
        var resultList = new List<string>();
        var spawnName = spawn.Name;

        if (count == 1)
        {
            resultList.Add(await _spawnRepository.Create(spawn));   
        }
        else
        {
            for (var i = 1; i <= count; i++)
            {
                spawn.Name = spawnName + "-" + i.ToString("D2");
                resultList.Add(await _spawnRepository.Create(spawn));
            }
        }

        return Created("", string.Join(",", resultList));
    }

    [HttpPut("{elementId}")]
    public async Task<IActionResult> Update
    (
        string  elementId,
        string? name,
        string? strain,
        string? notes,
        string? type,
        string? recipe,
        string? location,
        string? parent,
        string? parentType,
        string? child,
        string? childType,
        bool?   successful,
        bool?   finished,
        string  modifiedOn,
        string  modifiedBy
    )
    {
        if((parent == null && parentType != null ) || (parent != null && parentType == null))
            throw new ValidationException("If the Parent parameter has been provided, then the ParentType must also be provided");
        
        if((child == null && childType != null ) || (child != null && childType == null))
            throw new ValidationException("If the Child parameter has been provided, then the ChildType must also be provided");
        
        if (finished == null && successful != null)
            throw new ValidationException("When providing the Successful parameter, you must also specify the Finished parameter");
        
        var spawn = new Spawn()
        {
            Name       = name,
            Recipe     = recipe,
            Location   = location,
            Notes      = notes,
            Type       = type,
            Strain     = strain,
            Finished   = finished,
            Successful = successful,
            Parent     = parent,
            ParentType = parentType,
            Child      = child,
            ChildType  = childType,
            ModifiedOn = DateTime.Parse(modifiedOn),
            ModifiedBy = modifiedBy
        };

        var result = await _spawnRepository.Update(elementId, spawn);

        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var records = await _spawnRepository.GetAll();
        return Ok(records);
    }

    [HttpGet("id/{elementId}")]
    public async Task<IActionResult> GetById(string elementId) =>
        Ok(await _spawnRepository.GetById(new Spawn { ElementId = elementId}));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name) =>
        Ok(await _spawnRepository.GetByName(new Spawn { Name = name}));


    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name) =>
        Ok(await _spawnRepository.SearchByName(new Spawn { Name = name}));

    [HttpDelete("{elementId}")]
    public async Task<IActionResult> Delete(string elementId)
    {
        await _spawnRepository.Delete(elementId);
        return NoContent();
    }
}