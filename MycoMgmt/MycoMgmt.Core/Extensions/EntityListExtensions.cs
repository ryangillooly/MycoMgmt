using MycoMgmt.Domain.Models.DTO;
using Neo4j.Driver;

namespace MycoMgmt.Core.Helpers;

public static class EntityListExtensions
{
    public static List<NewNodeResultDTO> ToNodeList(this IEnumerable<IEntity> resultList) =>
        resultList
            .Where(entity => entity is INode)
            .Select(item => new NewNodeResultDTO
            {
                Name = item.Properties.TryGetValue("Name", out var name) ? (string?) name : null,
                Id   = item.Properties.TryGetValue("Id"  , out var id)   ? (string?) id   : null,
            })
            .ToList();
}