using MycoMgmt.Domain.Models;
using ErrorOr;
using MycoMgmt.Domain.Models.DTO;
using Neo4j.Driver;

namespace MycoMgmt.Infrastructure.Services;

public interface IActionService
{
    Task<List<NewNodeResult>> Create(ModelBase model, string url, int? count = 1);
    Task<List<NewNodeResult>> Update(ModelBase model, string url);
    void Delete(Guid id);
    Task<INode> GetById(ModelBase model);
}