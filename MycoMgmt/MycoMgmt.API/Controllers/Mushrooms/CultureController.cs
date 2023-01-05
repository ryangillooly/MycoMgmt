using Microsoft.AspNetCore.Mvc;
using MycoMgmt.API.Filters;
using MycoMgmt.Domain.Contracts.Mushroom.Culture;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.Infrastructure.Services;
using static MycoMgmt.Infrastructure.Helpers.IActionRepositoryExtensions;

namespace MycoMgmt.API.Controllers;

[Route("culture")]
[ApiController]
public class CultureController : BaseController<CultureController>
{
    private readonly IActionService _actionService;

    public CultureController(IActionService actionService)
    {
        _actionService = actionService;
    }
    
    [HttpPost]
    [MushroomValidation]
    public async Task<IActionResult> Create ([FromBody] CreateCultureRequest request)
    {
        var culture = new Culture
        (
            request.Name,
            request.Type,
            request.Notes,
            request.Strain,
            request.Location,
            request.Parent,
            request.ParentType,
            request.Children,
            request.ChildType,
            request.Successful,
            request.Finished,
            request.FinishedOn,
            request.InoculatedBy,
            request.InoculatedOn,
            request.CreatedBy,
            request.CreatedOn,
            request.Recipe,
            request.Purchased,
            request.Vendor
        );
        culture.Tags.Add(culture.IsSuccessful());
        culture.Status = culture.IsSuccessful();
        var result = await Repository.CreateEntities(Logger, culture);
        return Created("", result);
    }
    
    [HttpPut("{id:guid}")]
    [MushroomValidation]
    public async Task<IActionResult> Update ([FromBody] UpdateCultureRequest request, Guid id)
    {
        var culture = new Culture
        (
            request.Name,
            request.Type,
            request.Notes,
            request.Strain,
            request.Location,
            request.Parent,
            request.ParentType,
            request.Children,
            request.ChildType,
            request.Successful,
            (bool) request.Finished,
            request.FinishedOn,
            request.InoculatedBy,
            (DateTime) request.InoculatedOn,
            request.ModifiedBy,
            request.ModifiedOn,
            request.Recipe,
            request.Purchased,
            request.Vendor
        )
        {
            Id = id
        };
        return Ok(await Repository.Update(culture));
    }
    
    [HttpDelete("{Id}")]
    public async Task<IActionResult> Delete(Guid Id)
    {
        await Repository.Delete(new Culture { Id = Id });
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(int skip = 0, int limit = 20) => Ok(await Repository.GetAll(new Culture(), skip, limit));

    [HttpGet("id/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) => Ok(await Repository.GetById(new Culture { Id = id }));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name) => Ok(await Repository.GetByName(new Culture { Name = name }));

    [HttpGet("search/name/{name}")]
    public async Task<IActionResult> SearchByName(string name) => Ok(await Repository.SearchByName(new Culture { Name = name }));
}