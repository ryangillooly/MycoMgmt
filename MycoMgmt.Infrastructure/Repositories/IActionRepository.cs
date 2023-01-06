using MycoMgmt.Domain.Models;
using Neo4j.Driver;

namespace MycoMgmt.Infrastructure.Repositories;

public interface IActionRepository
{
    public Task<string> SearchByName(ModelBase model);
    public Task<string> GetByName(ModelBase model);
    public Task<INode> GetById(ModelBase model);
    public Task<string> GetAll(ModelBase model, int skip, int limit);
    public Task<List<IEntity>> Create(ModelBase model);
    public Task Delete(ModelBase model);
    public Task<List<IEntity>> Update(ModelBase model);
}