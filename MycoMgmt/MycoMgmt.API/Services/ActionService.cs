using MycoMgmt.Core.Extensions;
using MycoMgmt.Core.Models;
using MycoMgmt.Core.Models.DTO;
using MycoMgmt.Infrastructure.Helpers;
using MycoMgmt.Infrastructure.Repositories;

namespace MycoMgmt.Core.Services;

public class ActionService : IActionService
{
    private readonly ILogger<ActionService> _logger;
    private readonly IActionRepository _actionRepository;

    public ActionService(ILogger<ActionService> logger, IActionRepository  actionRepository)
    {
        _actionRepository = actionRepository;
        _logger = logger;
    }
    
    public async Task<List<NewNodeResult>> Create(ModelBase model, string url, int? count = 1)
    { 
        var results = await _actionRepository.CreateEntities(_logger, model, count);
        results.ForEach(i => i.Url = $"{url}/{i.Id}");
        
        return results;
    }

    public async Task<List<NewNodeResult>> Update(ModelBase model, string url)
    {
        var results = await _actionRepository.Update(model);
        var resultList = results.ToNodeList();
        resultList.ForEach(i => i.Url = $"{url}");
        return resultList;
    }

    public async void Delete(ModelBase model) => await _actionRepository.Delete(model);
    public async Task<GetNodeDto> GetById(ModelBase model) => await _actionRepository.GetById(model);
    public async Task<IEnumerable<GetNodeDto>> GetAll(ModelBase model, int skip, int limit) => await _actionRepository.GetAll(model, skip, limit); 
    public async Task<GetNodeDto> GetByName(ModelBase model) => await _actionRepository.GetByName(model); 
    public async Task<IEnumerable<GetNodeDto>> SearchByName(ModelBase model, int skip, int limit)  => await _actionRepository.SearchByName(model, skip, limit);
}