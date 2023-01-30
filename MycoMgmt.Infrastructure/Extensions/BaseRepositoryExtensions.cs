using Microsoft.Extensions.Logging;
using MycoMgmt.Core.Models;
using MycoMgmt.Infrastructure.Repositories;
using Neo4j.Driver;
using Newtonsoft.Json;
using MycoMgmt.Core.Helpers;
using MycoMgmt.Core.Models.DTO;
using MycoMgmt.Core.Models.Mushrooms;
using ILogger = Microsoft.Extensions.Logging.ILogger;


namespace MycoMgmt.Infrastructure.Helpers;

public static class ActionRepositoryExtensions
{
    public static async Task<List<NewNodeResult>> CreateEntities<T>(this IActionRepository repository, ILogger logger, T model, int? count = 1) where T : ModelBase
    {
        var resultList = new List<IEntity>();
        var modelName = model.Name;

        if (count == 1)
        {
            var results = await repository.Create(model);
            resultList = resultList.Concat(results).ToList();
        }
        else
        {
            for (var i = 1; i <= count; i++)
            {
                model.Id = Guid.NewGuid();
                model.Name = modelName + "-" + i.ToString("D2");
                var results = await repository.Create(model);
                resultList = resultList.Concat(results).ToList();
            }
        }
        
        var nodeList = resultList.ToNodeList();
        logger.LogInformation("New {Type}s Created - {CultureName}", model.GetType().Name, nodeList.Select(item => $"{item.Name} ({item.Id})"));
        
        return nodeList;
    }
}