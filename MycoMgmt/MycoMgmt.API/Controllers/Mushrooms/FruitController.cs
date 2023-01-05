using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Filters;
using MycoMgmt.Core.Helpers;
using MycoMgmt.Infrastructure.Repositories;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.Infrastructure.Helpers;

namespace MycoMgmt.API.Controllers;

[Route("fruit")]
[ApiController]
public class FruitController : BaseController<FruitController>
{
    [HttpPost]
    [MushroomValidation]
    public async Task<IActionResult> Create ([FromBody] Fruit mushroom, int? count = 1)
    {
        mushroom.Tags.Add(mushroom.IsSuccessful());
        mushroom.Status  = mushroom.IsSuccessful();
        var result  = await Repository.CreateEntities(Logger, mushroom, count);
        return Created("", string.Join(",", result));
    }

    [HttpPut("{id}")]
    [MushroomValidation]
    public async Task<IActionResult> Update([FromBody] Fruit mushroom, string id)
    {
        mushroom.Id = id;        
        return Ok(await Repository.Update(mushroom));
    }
    
    
    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete(string Id)
    {
        await Repository.Delete(new Fruit { Id = Id });
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await Repository.GetAll(new Fruit(), skip, limit));

    [HttpGet("id/{Id}")]
    public async Task<IActionResult> GetById(string Id) => Ok(await Repository.GetById(new Fruit { Id = Id }));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name) => Ok(await Repository.GetByName(new Fruit { Name = name }));

    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name) => Ok(await Repository.SearchByName(new Fruit { Name = name }));
}