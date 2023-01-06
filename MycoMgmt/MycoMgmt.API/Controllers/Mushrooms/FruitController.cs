using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Filters;
using MycoMgmt.Core.Helpers;
using MycoMgmt.Domain.Contracts.Mushroom;
using MycoMgmt.Infrastructure.Repositories;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.Infrastructure.Helpers;

namespace MycoMgmt.API.Controllers;

[Route("[controller]")]
[ApiController]
public class FruitController : BaseController<FruitController>
{
    [HttpPost]
    [MushroomValidation]
    public async Task<IActionResult> Create ([FromBody] CreateMushroomRequest request)
    {
        var fruit = new Fruit
        (
            request.Name,
            request.Strain,
            request.WetWeight,
            request.DryWeight,
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
            request.Finished
        )
        {
            CreatedOn   = DateTime.Now,
            CreatedBy   = request.CreatedBy,
            HarvestedOn = request.HarvestedOn,
            HarvestedBy = request.HarvestedBy
        };
        
        fruit.Tags.Add(fruit.IsSuccessful());
        fruit.Status = fruit.IsSuccessful();
        
        var result = await ActionService.Create
        (
            fruit, 
            HttpContext.Request.GetDisplayUrl(), 
            request.Count
        );
        
        return Created("", result);
    }

    [HttpPut("{id:guid}")]
    [MushroomValidation]
    public async Task<IActionResult> Update ([FromBody] CreateMushroomRequest request, Guid id)
    {
        var fruit = new Fruit
        (
            request.Name,
            request.Strain,
            request.WetWeight,
            request.DryWeight,
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
            request.Finished
        )
        {
            Id         = id,
            ModifiedOn = DateTime.Now,
            ModifiedBy = request.ModifiedBy,
            HarvestedOn = request.HarvestedOn,
            HarvestedBy = request.HarvestedBy
        };

        return Ok(await Repository.Update(fruit));
    }
    
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Repository.Delete(new Fruit { Id = id });
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await Repository.GetAll(new Fruit(), skip, limit));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) => Ok(await Repository.GetById(new Fruit { Id = id }));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name) => Ok(await Repository.GetByName(new Fruit { Name = name }));

    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name) => Ok(await Repository.SearchByName(new Fruit { Name = name }));
}