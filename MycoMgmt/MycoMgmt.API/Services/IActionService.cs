using MycoMgmt.Core.Models;
using MycoMgmt.Core.Models.DTO;
using Neo4j.Driver;

namespace MycoMgmt.Core.Services;

public interface IActionService
{
    Task<List<NewNodeResult>> Create(ModelBase model, string url, int? count = 1);
    Task<List<NewNodeResult>> Update(ModelBase model, string url);
    void Delete(Guid id);
    Task<GetNodeDto> GetById(ModelBase model);
    Task<IEnumerable<GetNodeDto>> GetAll(ModelBase model, int skip, int limit);
}