using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Filters;
using MycoMgmt.API.Models;
using MycoMgmt.Core.Helpers;
using MycoMgmt.Infrastructure.Repositories;
using MycoMgmt.Domain.Models.Mushrooms;
using static MycoMgmt.Infrastructure.Helpers.IActionRepositoryExtensions;

namespace MycoMgmt.API.Controllers;

[Route("culture")]
[ApiController]
public class CultureController : BaseController<CultureController>
{
    [HttpPost]
    [MushroomValidation]
    public async Task<IActionResult> Create ([FromBody] Culture mushroom, int? count = 1)
    {
        mushroom.Tags.Add(mushroom.IsSuccessful());
        mushroom.Status = mushroom.IsSuccessful();
        var result = await Repository.CreateEntities(Logger, mushroom , count);
        return Created("", result);
    }
    
    [HttpPut("{id}")]
    [MushroomValidation]
    public async Task<IActionResult> Update ([FromBody] Culture mushroom, string id)
    {
        mushroom.Id = id;
        return Ok(await Repository.Update(mushroom));
    }
    
    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete(string Id)
    {
        await Repository.Delete(new Culture { Id = Id });
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(int skip = 0, int limit = 20) => Ok(await Repository.GetAll(new Culture(), skip, limit));

    [HttpGet("id/{Id}")]
    public async Task<IActionResult> GetById(string Id) => Ok(await Repository.GetById(new Culture { Id = Id }));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name) => Ok(await Repository.GetByName(new Culture { Name = name }));

    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name) => Ok(await Repository.SearchByName(new Culture { Name = name }));
}