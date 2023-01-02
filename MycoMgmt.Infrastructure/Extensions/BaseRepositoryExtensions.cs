using Microsoft.Extensions.Logging;
using MycoMgmt.Domain.Models;
using MycoMgmt.Infrastructure.Repositories;
using Neo4j.Driver;
using Newtonsoft.Json;
using MycoMgmt.Core.Helpers;
using ILogger = Microsoft.Extensions.Logging.ILogger;


namespace MycoMgmt.Infrastructure.Helpers;

public static class BaseRepositoryExtensions
{
    public static async Task<string> CreateEntities<T>(this BaseRepository<T> repository, ILogger logger, T model, int? count = 1) where T : ModelBase
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
                model.Id = Guid.NewGuid().ToString();
                model.Name = modelName + "-" + i.ToString("D2");
                var results = await repository.Create(model);
                resultList = resultList.Concat(results).ToList();
            }
        }
        
        var nodeList = resultList.ToNodeList();
        var result = JsonConvert.SerializeObject(nodeList);
        
        logger.LogInformation("New {Type}s Created - {CultureName}", model.GetType().Name, nodeList.Select(item => $"{item.Name} ({item.Id})"));
        
        return result;
    }
}