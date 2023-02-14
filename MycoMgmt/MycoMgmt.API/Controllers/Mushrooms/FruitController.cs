using AutoMapper;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Filters;
using MycoMgmt.API.Helpers;
using MycoMgmt.Core.Contracts.Mushroom;
using MycoMgmt.Core.Models.Mushrooms;

namespace MycoMgmt.API.Controllers;

[Route("[controller]")]
[ApiController]
public class FruitController : BaseController<FruitController>
{
    [HttpPost]
    [MushroomValidation]
    public async Task<IActionResult> Create ([FromBody] CreateMushroomRequest request) => Created("", await request.Create<Fruit>(Mapper, ActionService, HttpContext.Request.GetDisplayUrl()));

    [HttpPut("{id:guid}")]
    [MushroomValidation] 
    public async Task<IActionResult> Update ([FromBody] UpdateMushroomRequest request, Guid id) => Ok(await request.Update<Fruit>(Mapper, ActionService, HttpContext.Request.GetDisplayUrl(), id));
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        ActionService.Delete(new Fruit { Id = id });
        return NoContent();
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) => 
        Ok(await ActionService.GetById(new Fruit { Id = id }));

    [HttpGet]
    public async Task<IActionResult> GetAll(int skip = 0, int limit = 20) => 
        Ok(await ActionService.GetAll(new Fruit(), skip, limit));
    
    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name) => 
        Ok(await ActionService.GetByName(new Fruit { Name = name }));
   
    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name, int skip = 0, int limit = 20) => 
        Ok(await ActionService.SearchByName(new Fruit { Name = name }, skip, limit));
}