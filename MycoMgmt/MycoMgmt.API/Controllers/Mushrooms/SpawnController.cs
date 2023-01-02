using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Infrastructure.Repositories;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.Infrastructure.Helpers;

namespace MycoMgmt.API.Controllers;

[Route("spawn")]
[ApiController]
public class SpawnController : Controller
{
    private readonly BaseRepository<Spawn> _spawnRepository;
    private readonly ILogger<SpawnController> _logger;

    public SpawnController(BaseRepository<Spawn> repo, ILogger<SpawnController> logger)
    {
        _spawnRepository = repo;
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
        if((parent == null && parentType != null ) || (parent != null && parentType == null))
            throw new ValidationException("If the Parent parameter has been provided, then the ParentType must also be provided");
        
        if((child == null && childType != null ) || (child != null && childType == null))
            throw new ValidationException("If the Children parameter has been provided, then the ChildType must also be provided");

        var spawn = new Spawn()
        {
            Name         = name,
            Recipe       = recipe,
            Location     = location,
            Notes        = notes,
            Type         = type,
            Strain       = strain,
            Successful   = successful,
            Parent       = parent,
            ParentType   = parentType,
            Children     = child,
            ChildType    = childType,
            Vendor       = vendor,
            Finished     = finished,
            FinishedOn   = finishedOn is null ? null : DateTime.Parse(finishedOn),
            InoculatedOn = inoculatedOn is null ? null : DateTime.Parse(inoculatedOn),
            InoculatedBy = inoculatedBy,
            CreatedOn    = DateTime.Parse(createdOn),
            CreatedBy    = createdBy
        };
        
        spawn.Tags.Add(spawn.IsSuccessful());
        spawn.Status  = spawn.IsSuccessful();
        
        var result  = await _spawnRepository.CreateEntities(_logger, spawn, count);

        return Created("", string.Join(",", result));
    }

    [HttpPut("{Id}")]
    public async Task<IActionResult> Update
    (
        string  Id,
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
        
        var spawn = new Spawn()
        {
            Id    = Id,
            Name         = name,
            Recipe       = recipe,
            Location     = location,
            Notes        = notes,
            Type         = type,
            Strain       = strain,
            Successful   = successful,
            Parent       = parent,
            ParentType   = parentType,
            Children     = child,
            ChildType    = childType,
            Vendor       = vendor, 
            Finished     = finished,
            FinishedOn   = finishedOn is null ? null : DateTime.Parse(finishedOn),
            InoculatedOn = inoculatedOn is null ? null : DateTime.Parse(inoculatedOn),
            InoculatedBy = inoculatedBy,
            ModifiedOn   = DateTime.Parse(modifiedOn),
            ModifiedBy   = modifiedBy
        };

        return Ok(await _spawnRepository.Update(spawn));
    }
    
    
    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete(string Id)
    {
        await _spawnRepository.Delete(new Spawn { Id = Id });
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await _spawnRepository.GetAll(new Spawn(), skip, limit));

    [HttpGet("id/{Id}")]
    public async Task<IActionResult> GetById(string Id) => Ok(await _spawnRepository.GetById(new Spawn { Id = Id}));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name) => Ok(await _spawnRepository.GetByName(new Spawn { Name = name}));

    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name) => Ok(await _spawnRepository.SearchByName(new Spawn { Name = name}));
}