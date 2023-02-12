using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Filters;
using MycoMgmt.Core.Contracts.Mushroom;
using MycoMgmt.Core.Models.Mushrooms;

namespace MycoMgmt.API.Controllers;

[Route("[controller]")]
[ApiController]
public class BulkController : BaseController<BulkController>
{
    [HttpPost]
    [MushroomValidation]
    public async Task<IActionResult> Create ([FromBody] CreateMushroomRequest request)
    {
        var bulk = new Bulk
        (
            request.Name!,
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
        
        bulk.Tags.Add(bulk.IsSuccessful());
        bulk.Status = bulk.IsSuccessful();

        var url = HttpContext.Request.GetDisplayUrl();
        var result = await ActionService.Create(bulk, url, request.Count);
        
        return Created("", result);
    }

    [HttpPut("{id:guid}")]
    [MushroomValidation]
    public async Task<IActionResult> Update ([FromBody] CreateMushroomRequest request, Guid id)
    {
        var bulk = new Bulk
        (
            request.Name!,
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

        return Ok(await Repository.Update(bulk));
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Repository.Delete(new Bulk { Id = id });
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(int skip, int limit) => Ok(await Repository.GetAll(new Bulk(), skip, limit));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) => Ok(await Repository.GetById(new Bulk { Id = id }));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name) => Ok(await Repository.GetByName(new Bulk { Name = name }));

    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name, int skip = 0, int limit = 20) => Ok(await Repository.SearchByName(new Bulk { Name = name }, skip, limit));
}