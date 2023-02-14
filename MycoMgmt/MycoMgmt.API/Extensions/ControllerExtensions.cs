using AutoMapper;
using MycoMgmt.Core.Contracts;
using MycoMgmt.Core.Contracts.Mushroom;
using MycoMgmt.Core.Models;
using MycoMgmt.Core.Models.DTO;
using MycoMgmt.Core.Services;

namespace MycoMgmt.API.Helpers;

public static class ControllerExtensions
{
    public static async Task<List<NewNodeResult>> Create<T>(this CreateRequest request, IMapper mapper, IActionService service, string url) where T : ModelBase
    {
        var entity = mapper.Map<T>(request);
        var result = await service.Create(entity, url, request.Count);
        return result;
    }
    
    public static async Task<List<NewNodeResult>> Update<T>(this UpdateRequest request, IMapper mapper, IActionService service, string url, Guid id) where T : ModelBase
    {
        request.Id = id;
        var entity = mapper.Map<T>(request);
        var result = await service.Update(entity, url);
        return result;
    }
}


