using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Repositories;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.API.Helpers;
using Neo4j.Driver;
using Newtonsoft.Json;

namespace MycoMgmt.API.Controllers;

[Route("spawn")]
[ApiController]
public class SpawnController : Controller
{
    private readonly ISpawnRepository _spawnRepository;
    private readonly ILogger<SpawnController> _logger;

    public SpawnController(ISpawnRepository repo, ILogger<SpawnController> logger)
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
        
        var resultList = new List<IEntity>();
        var spawnName = spawn.Name;

        if (count == 1)
        {
            var results = await _spawnRepository.Create(spawn);
            resultList = resultList.Concat(results).ToList();
        }
        else
        {
            for (var i = 1; i <= count; i++)
            {
                spawn.Name = spawnName + "-" + i.ToString("D2");
                var results = await _spawnRepository.Create(spawn);
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


        return Created("", string.Join(",", result));
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
            ElementId    = elementId,
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
    
    
    [HttpDelete("{elementId}")]
    public async Task<IActionResult> Delete(string elementId)
    {
        await _spawnRepository.Delete(new Spawn { ElementId = elementId });
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await _spawnRepository.GetAll(new Spawn(), skip, limit));

    [HttpGet("id/{elementId}")]
    public async Task<IActionResult> GetById(string elementId) => Ok(await _spawnRepository.GetById(new Spawn { ElementId = elementId}));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name) => Ok(await _spawnRepository.GetByName(new Spawn { Name = name}));

    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name) => Ok(await _spawnRepository.SearchByName(new Spawn { Name = name}));
}