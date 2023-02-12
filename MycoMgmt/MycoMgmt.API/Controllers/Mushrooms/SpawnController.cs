using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Filters;
using MycoMgmt.Core.Contracts.Mushroom;
using MycoMgmt.Core.Models.Mushrooms;
using MycoMgmt.API.Helpers;

namespace MycoMgmt.API.Controllers;

[Route("[controller]")]
[ApiController]
public class SpawnController : BaseController<SpawnController>
{
    [HttpPost]
    [MushroomValidation]
    public async Task<IActionResult> Create ([FromBody] CreateMushroomRequest request) => Created("", await request.Create<Spawn>(Mapper, ActionService, HttpContext.Request.GetDisplayUrl()));

    [HttpPut("{id:guid}")]
    [MushroomValidation]
    public async Task<IActionResult> Update ([FromBody] UpdateMushroomRequest request, Guid id) => Ok(await request.Update<Spawn>(Mapper, ActionService, HttpContext.Request.GetDisplayUrl(), id));

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
    public async Task<IActionResult> SearchByName(string name, int skip = 0, int limit = 20) => Ok(await Repository.SearchByName(new Spawn { Name = name}, skip, limit));
}