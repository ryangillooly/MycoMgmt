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

    [HttpPut("{id:guid}")]
    [MushroomValidation]
    public async Task<IActionResult> Update([FromBody] Fruit mushroom, Guid id)
    {
        mushroom.Id = id;        
        return Ok(await Repository.Update(mushroom));
    }
    
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Repository.Delete(new Fruit { Id = id });
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await Repository.GetAll(new Fruit(), skip, limit));

    [HttpGet("id/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) => Ok(await Repository.GetById(new Fruit { Id = id }));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name) => Ok(await Repository.GetByName(new Fruit { Name = name }));

    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name) => Ok(await Repository.SearchByName(new Fruit { Name = name }));
}