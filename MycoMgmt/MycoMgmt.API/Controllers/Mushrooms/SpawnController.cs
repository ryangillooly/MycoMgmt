using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Repositories;
using MycoMgmt.Domain.Models.Mushrooms;

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
        var spawn = new Spawn()
        {
            Name = name,
            Type = type,
            Strain = strain,
            Finished = finished,
            CreatedOn = DateTime.Parse(createdOn),
            CreatedBy = createdBy
        };

        if (recipe != null)
            spawn.Recipe = recipe;

        if (location != null)
            spawn.Location = location;
        
        if (notes != null)
            spawn.Notes = notes;

        if((parent == null && parentType != null ) || (parent != null && parentType == null))
            throw new ValidationException("If the Parent parameter has been provided, then the ParentType must also be provided");
        
        if((child == null && childType != null ) || (child != null && childType == null))
            throw new ValidationException("If the Child parameter has been provided, then the ChildType must also be provided");
        
        if (parent != null && parentType != null)
        {
            spawn.Parent     = parent;
            spawn.ParentType = parentType;
        }
        
        if (child != null && childType != null)
        {
            spawn.Child     = child;
            spawn.ChildType = childType;
        }

        if (successful != null)
            spawn.Successful = successful.Value;

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
        string elementId,
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
        bool? successful,
        bool? finished,
        string modifiedOn,
        string modifiedBy
    )
    {
        var spawn = new Spawn
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
            spawn.Name = name;

        if (notes != null)
            spawn.Notes = notes;
        
        if (strain != null)
            spawn.Strain = strain;
        
        if (type != null)
            spawn.Type = type;
        
        if (recipe != null)
            spawn.Recipe = recipe;

        if (location != null)
            spawn.Location = location;

        if (parent != null && parentType != null)
        {
            spawn.Parent     = parent;
            spawn.ParentType = parentType;
        }
        
        if (child != null && childType != null)
        {
            spawn.Child     = child;
            spawn.ChildType = childType;
        }

        if (successful != null)
            spawn.Successful = successful.Value;

        if (finished != null)
            spawn.Finished = finished;

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
    public async Task<IActionResult> GetById(string elementId)
    {
        var results = await _spawnRepository.GetById(elementId);
        return Ok(results);
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        var results = await _spawnRepository.GetByName(name);
        return Ok(results);
    }


    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name)
    {
        var results = await _spawnRepository.SearchByName(name);
        return Ok(results);
    }

    [HttpDelete("{elementId}")]
    public async Task<IActionResult> Delete(string elementId)
    {
        await _spawnRepository.Delete(elementId);
        return NoContent();
    }
}