using MycoMgmt.Domain.Models;
using ErrorOr;
using Microsoft.Extensions.Logging;
using MycoMgmt.Core.Helpers;
using MycoMgmt.Domain.Models.DTO;
using MycoMgmt.Infrastructure.Helpers;
using MycoMgmt.Infrastructure.Repositories;
using Neo4j.Driver;
using Newtonsoft.Json;

namespace MycoMgmt.Infrastructure.Services;

public class ActionService : IActionService
{
    private readonly ILogger<ActionService> _logger;
    private readonly IActionRepository _actionRepository;

    public ActionService(ILogger<ActionService> logger,IActionRepository  actionRepository)
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
        resultList.ForEach(i => i.Url = $"{url}/{i.Id}");
        return resultList;
    }

    public void Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<INode> GetById(ModelBase model) => await _actionRepository.GetById(model);
}