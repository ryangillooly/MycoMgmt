using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Filters;
using MycoMgmt.Core.Extensions;
using MycoMgmt.Core.Contracts.Mushroom;
using MycoMgmt.Infrastructure.Repositories;
using MycoMgmt.Core.Models.Mushrooms;
using MycoMgmt.Infrastructure.Helpers;

namespace MycoMgmt.API.Controllers;

[Route("[controller]")]
[ApiController]
public class SpawnController : BaseController<SpawnController>
{
    [HttpPost]
    [MushroomValidation]
    public async Task<IActionResult> Create ([FromBody] CreateMushroomRequest request)
    {
        var spawn = new Spawn
        (
            request.Name!,
            request.Type!,
            request.Strain!,
            request.Recipe,
            request.Notes,
            request.Location,
            request.Parent,
            request.ParentType,
            request.Children,
            request.ChildType,
            request.Vendor,
            request.Purchased,
            request.Successful,
            request.Finished,
            request.FinishedOn,
            request.InoculatedOn,
            request.InoculatedBy
        )
        {
            CreatedOn = DateTime.Now,
            CreatedBy = request.CreatedBy
        };
        
        spawn.Tags.Add(spawn.IsSuccessful());
        spawn.Status = spawn.IsSuccessful();
        
        var url = HttpContext.Request.GetDisplayUrl();
        var result = await ActionService.Create(spawn, url, request.Count);
        
        return Created("", result);
    }

    [HttpPut("{id:guid}")]
    [MushroomValidation]
    public async Task<IActionResult> Update ([FromBody] CreateMushroomRequest request, Guid id)
    {
        var spawn = new Spawn
        (
            request.Name!,
            request.Type!,
            request.Strain!,
            request.Recipe,
            request.Notes,
            request.Location,
            request.Parent,
            request.ParentType,
            request.Children,
            request.ChildType,
            request.Vendor,
            request.Purchased,
            request.Successful,
            request.Finished,
            request.FinishedOn,
            request.InoculatedOn,
            request.InoculatedBy
        )
        {
            Id         = id,
            ModifiedOn = DateTime.Now,
            ModifiedBy = request.ModifiedBy
        };

        return Ok(await Repository.Update(spawn));
    }
    
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Repository.Delete(new Spawn { Id = id });
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await Repository.GetAll(new Spawn(), skip, limit));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) => Ok(await Repository.GetById(new Spawn { Id = id}));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name) => Ok(await Repository.GetByName(new Spawn { Name = name}));

    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name) => Ok(await Repository.SearchByName(new Spawn { Name = name}));
}