using MycoMgmt.Domain.Models;
using Neo4j.Driver;

namespace MycoMgmt.Infrastructure.Repositories;

public interface IActionRepository
{
    public Task<string> SearchByName<T>(T model) where T : ModelBase;
    public Task<string> GetByName<T>(T model) where T : ModelBase;
    public Task<INode> GetById<T>(T model) where T : ModelBase;
    public Task<string> GetAll<T>(T model, int skip, int limit) where T : ModelBase;
    public Task<List<IEntity>> Create<T>(T model) where T : ModelBase;
    public Task Delete<T>(T model) where T : ModelBase;
    public Task<List<IEntity>> Update<T>(T model) where T : ModelBase;
}