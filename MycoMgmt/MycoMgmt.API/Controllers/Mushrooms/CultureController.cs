using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Filters;
using MycoMgmt.Domain.Contracts.Mushroom;
using MycoMgmt.Domain.Models.Mushrooms;

namespace MycoMgmt.API.Controllers;

[Route("[controller]")]
[ApiController]
public class CultureController : BaseController<CultureController>
{
    [HttpPost]
    [MushroomValidation]
    public async Task<IActionResult> Create ([FromBody] CreateMushroomRequest request)
    {
        var culture = new Culture
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
        culture.Tags.Add(culture.IsSuccessful());
        culture.Status = culture.IsSuccessful();

        var url = HttpContext.Request.GetDisplayUrl();
        var result = await ActionService.Create(culture, url, request.Count);
   
       return Created("", result);
    }
    
    [HttpPut("{id:guid}")]
    [MushroomValidation]
    public async Task<IActionResult> Update ([FromBody] CreateMushroomRequest request, Guid id)
    {
        var culture = new Culture
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
        
        return Ok(await Repository.Update(culture));
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Repository.Delete(new Culture { Id = id });
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(int skip = 0, int limit = 20) => Ok(await Repository.GetAll(new Culture(), skip, limit));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) => Ok(await ActionService.GetById(new Culture { Id = id }));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name) => Ok(await Repository.GetByName(new Culture { Name = name }));

    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name) => Ok(await Repository.SearchByName(new Culture { Name = name }));
}